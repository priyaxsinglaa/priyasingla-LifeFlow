using API.DTOs.Forecast;
using API.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("api/forecast")]
public class ForecastController : ControllerBase
{
    private readonly IForecastingService _forecastService;
    private readonly IBloodStockService _stockService;

    // Both services injected — forecast handles AI logic, stock provides blood type names
    public ForecastController(IForecastingService forecastService, IBloodStockService stockService)
    {
        _forecastService = forecastService;
        _stockService = stockService;
    }

    // ── POST /api/forecast ───────────────────────────────────────────────────
    // Runs the Ollama AI demand forecast for a given blood type + hospital
    // Body: { "bloodType": "A+", "hospital": "City Hospital", "daysAhead": 7 }
    [HttpPost]
    public async Task<IActionResult> Forecast([FromBody] ForecastRequestDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        // Guard: daysAhead must be between 1 and 90
        if (dto.DaysAhead < 1 || dto.DaysAhead > 90)
            return BadRequest(new { message = "daysAhead must be between 1 and 90." });

        var result = await _forecastService.ForecastAsync(dto);
        return Ok(result);
    }

    // ── GET /api/forecast/dashboard ──────────────────────────────────────────
    // Returns KPI cards for the dashboard:
    // total donations this month, total units, active alerts, critical alerts, stock summary
    [HttpGet("dashboard")]
    public async Task<IActionResult> Dashboard()
    {
        var kpis = await _forecastService.GetDashboardKpisAsync();
        return Ok(kpis);
    }

    // ── GET /api/forecast/blood-types ────────────────────────────────────────
    // Returns list of blood type strings e.g. ["A+","A-","B+","B-","AB+","AB-","O+","O-"]
    // Used by the frontend to populate dropdowns without hardcoding them
    [HttpGet("blood-types")]
    public async Task<IActionResult> GetBloodTypes()
    {
        var types = await _stockService.GetBloodTypeNamesAsync();
        return Ok(types);
    }
}