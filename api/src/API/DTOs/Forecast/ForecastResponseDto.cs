namespace API.DTOs.Forecast;

public class ForecastResponseDto
{
    public string BloodType { get; set; } = null!;
    public string Hospital { get; set; } = null!;
    public List<DailyForecast> Predictions { get; set; } = new();
    public string? AiInsight { get; set; }
}

public class DailyForecast
{
    public DateTime Date { get; set; }
    public float PredictedUnits { get; set; }
    public float LowerBound { get; set; }
    public float UpperBound { get; set; }
}

public class DashboardKpiDto
{
    public int TotalDonationsThisMonth { get; set; }
    public int TotalUnitsThisMonth { get; set; }
    public int ActiveAlerts { get; set; }
    public int CriticalAlerts { get; set; }
    public List<BloodStockSummary> StockSummary { get; set; } = new();
}

public class BloodStockSummary
{
    public string BloodType { get; set; } = null!;
    public int AvailableUnits { get; set; }
    public string Status { get; set; } = null!;
}