using API.DTOs.Forecast;

namespace API.Services.Interfaces;

public interface IForecastingService
{
    Task<ForecastResponseDto> ForecastAsync(ForecastRequestDto request);
    Task<DashboardKpiDto> GetDashboardKpisAsync();
}