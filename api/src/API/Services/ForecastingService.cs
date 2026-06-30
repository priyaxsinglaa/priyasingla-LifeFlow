using API.DTOs.Forecast;
using API.Models;
using API.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace API.Services;

public class ForecastingService : IForecastingService
{
    private readonly LifeFlowDbContext _db;
    private readonly HttpClient _http;
    private readonly ILogger<ForecastingService> _logger;

    private const string OllamaUrl = "http://localhost:11434/api/chat"; // chat endpoint handles Qwen3 better
    private const string Model = "qwen3.5:2b-q4_K_M";

    public ForecastingService(LifeFlowDbContext db, IHttpClientFactory httpClientFactory,
        ILogger<ForecastingService> logger)
    {
        _db = db;
        _http = httpClientFactory.CreateClient();
        _logger = logger;
        _http.Timeout = TimeSpan.FromSeconds(120);
    }

    // ── POST /api/forecast ───────────────────────────────────────────────────

    public async Task<ForecastResponseDto> ForecastAsync(ForecastRequestDto request)
    {
        var history = await _db.DemandForecasts
            .Where(d => d.BloodType == request.BloodType && d.Hospital == request.Hospital)
            .OrderBy(d => d.ForecastDate)
            .Select(d => d.PredictedUnits)
            .ToListAsync();

        if (history.Count < 5)
        {
            _logger.LogWarning("Not enough history for {BloodType} at {Hospital}. Count: {Count}",
                request.BloodType, request.Hospital, history.Count);
            var avg = history.Any() ? (float)history.Average() : 10f;
            return BuildSimpleForecast(request, avg);
        }

        var (predictions, insight) = await AskOllamaForForecastAsync(
            history, request.DaysAhead, request.BloodType, request.Hospital);

        return new ForecastResponseDto
        {
            BloodType = request.BloodType,
            Hospital = request.Hospital,
            Predictions = predictions,
            AiInsight = insight
        };
    }

    // ── GET /api/forecast/dashboard ──────────────────────────────────────────

    public async Task<DashboardKpiDto> GetDashboardKpisAsync()
    {
        var startOfMonth = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);

        var donations = await _db.Donations
            .Where(d => d.DonationDate >= startOfMonth)
            .ToListAsync();

        var activeAlerts = await _db.ShortageAlerts.CountAsync(a => a.IsActive);
        var criticalAlerts = await _db.ShortageAlerts
            .CountAsync(a => a.IsActive && a.Severity == "Critical");

        var stockSummary = await _db.BloodStocks
            .Select(s => new BloodStockSummary
            {
                BloodType = s.BloodType,
                AvailableUnits = s.AvailableUnits,
                Status = s.Status
            })
            .ToListAsync();

        return new DashboardKpiDto
        {
            TotalDonationsThisMonth = donations.Count,
            TotalUnitsThisMonth = donations.Sum(d => d.Units),
            ActiveAlerts = activeAlerts,
            CriticalAlerts = criticalAlerts,
            StockSummary = stockSummary
        };
    }

    // ── Ollama /api/chat call ────────────────────────────────────────────────
    // Using /api/chat instead of /api/generate because:
    // - chat endpoint supports "think": false natively for Qwen3
    // - response comes in message.content instead of response field
    // - more reliable structured output

    private async Task<(List<DailyForecast> predictions, string insight)> AskOllamaForForecastAsync(
        List<int> history, int daysAhead, string bloodType, string hospital)
    {
        var historyText = string.Join(", ", history);

        // System message sets strict JSON-only behaviour
        // User message gives the actual task
        // think: false disables Qwen3 chain-of-thought at the API level (works on all Qwen3 builds)
        var requestBody = new
        {
            model = Model,
            stream = false,
            think = false,          // Qwen3-specific: disables <think> block entirely
            format = "json",
            options = new
            {
                temperature = 0.1,   // near-deterministic output, better for numbers
                num_predict = 600    // enough for 90-day forecast JSON
            },
            messages = new[]
            {
                new
                {
                    role    = "system",
                    content = "You are a blood demand forecasting assistant. You only output valid JSON. No thinking. No explanations. No markdown."
                },
                new
                {
                    role    = "user",
                    content = $"Blood type {bloodType} at {hospital}. " +
                              $"Historical monthly demand (oldest to newest): [{historyText}]. " +
                              $"Predict next {daysAhead} days. " +
                              $"Reply with ONLY this JSON structure: " +
                              $"{{\"predictions\":[{{\"day\":1,\"units\":12.5,\"lower\":10.0,\"upper\":15.0}}],\"insight\":\"trend summary\"}}"
                }
            }
        };

        string rawResponse = string.Empty;

        try
        {
            var json = JsonSerializer.Serialize(requestBody);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _http.PostAsync(OllamaUrl, content);
            response.EnsureSuccessStatusCode();
            rawResponse = await response.Content.ReadAsStringAsync();
            _logger.LogInformation("Ollama raw: {Raw}", rawResponse);
        }
        catch (Exception ex)
        {
            _logger.LogError("Ollama HTTP failed: {Err}", ex.Message);
            return FallbackResult(history, "Ollama unavailable — showing average-based estimate.");
        }

        // ── Parse Ollama chat envelope ───────────────────────────────────────
        // /api/chat response shape:
        // { "message": { "role": "assistant", "content": "{...your JSON...}" }, "done": true }

        OllamaChatResponse? envelope;
        try
        {
            envelope = JsonSerializer.Deserialize<OllamaChatResponse>(rawResponse,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }
        catch (Exception ex)
        {
            _logger.LogError("Envelope parse failed: {Err}\nRaw: {Raw}", ex.Message, rawResponse);
            return FallbackResult(history, "Could not parse Ollama response.");
        }

        if (!string.IsNullOrWhiteSpace(envelope?.Error))
        {
            _logger.LogError("Ollama error field: {Err}", envelope.Error);
            return FallbackResult(history, $"Ollama error: {envelope.Error}");
        }

        var innerJson = envelope?.Message?.Content?.Trim() ?? string.Empty;
        _logger.LogInformation("Ollama inner content: {Inner}", innerJson);

        if (string.IsNullOrWhiteSpace(innerJson))
        {
            _logger.LogWarning("Ollama message.content is empty.");
            return FallbackResult(history, "Model returned empty content.");
        }

        // Strip markdown fences if model still added them despite instructions
        innerJson = StripMarkdownFences(innerJson);

        // ── Parse forecast JSON ──────────────────────────────────────────────
        OllamaForecastResult? result;
        try
        {
            result = JsonSerializer.Deserialize<OllamaForecastResult>(innerJson,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }
        catch (Exception ex)
        {
            _logger.LogError("Forecast JSON parse failed: {Err}\nInner: {Inner}", ex.Message, innerJson);
            return FallbackResult(history, "Model returned unreadable JSON.");
        }

        if (result?.Predictions == null || result.Predictions.Count == 0)
        {
            _logger.LogWarning("No predictions in parsed result. Inner: {Inner}", innerJson);
            return FallbackResult(history, "Model returned no predictions.");
        }

        var predictions = result.Predictions.Select(p => new DailyForecast
        {
            Date = DateTime.Today.AddDays(p.Day),
            PredictedUnits = Math.Max(0, p.Units),
            LowerBound = Math.Max(0, p.Lower),
            UpperBound = p.Upper
        }).ToList();

        return (predictions, result.Insight ?? "AI forecast generated.");
    }

    // ── Helpers ──────────────────────────────────────────────────────────────

    private static string StripMarkdownFences(string text)
    {
        if (!text.Contains("```") && text.TrimStart().StartsWith("{"))
            return text; // already clean JSON

        var start = text.IndexOf('{');
        var end = text.LastIndexOf('}');
        return (start >= 0 && end > start) ? text[start..(end + 1)] : text;
    }

    private (List<DailyForecast>, string) FallbackResult(List<int> history, string reason)
    {
        _logger.LogWarning("Using fallback: {Reason}", reason);
        var avg = history.Any() ? (float)history.Average() : 10f;
        var fallback = BuildSimpleForecast(new ForecastRequestDto { DaysAhead = 7 }, avg);
        return (fallback.Predictions, reason);
    }

    private static ForecastResponseDto BuildSimpleForecast(ForecastRequestDto req, float avg)
    {
        var days = req.DaysAhead < 1 ? 7 : req.DaysAhead;
        return new ForecastResponseDto
        {
            BloodType = req.BloodType,
            Hospital = req.Hospital,
            Predictions = Enumerable.Range(1, days).Select(i => new DailyForecast
            {
                Date = DateTime.Today.AddDays(i),
                PredictedUnits = avg,
                LowerBound = avg * 0.8f,
                UpperBound = avg * 1.2f
            }).ToList(),
            AiInsight = "Insufficient historical data. Showing average-based estimate."
        };
    }

    // ── Ollama /api/chat response DTOs ───────────────────────────────────────

    private class OllamaChatResponse
    {
        [JsonPropertyName("message")] public OllamaChatMessage? Message { get; set; }
        [JsonPropertyName("done")] public bool Done { get; set; }
        [JsonPropertyName("error")] public string? Error { get; set; }
    }

    private class OllamaChatMessage
    {
        [JsonPropertyName("role")] public string? Role { get; set; }
        [JsonPropertyName("content")] public string? Content { get; set; }
    }

    private class OllamaForecastResult
    {
        public List<OllamaDayPrediction> Predictions { get; set; } = new();
        public string? Insight { get; set; }
    }

    private class OllamaDayPrediction
    {
        public int Day { get; set; }
        public float Units { get; set; }
        public float Lower { get; set; }
        public float Upper { get; set; }
    }
}