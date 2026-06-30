using API.DTOs.Alert;

namespace API.Services.Interfaces;

public interface IAlertService
{
    Task<IEnumerable<AlertResponseDto>> GetActiveAlertsAsync();
    Task<IEnumerable<AlertResponseDto>> EvaluateAndCreateAlertsAsync();
    Task<AlertResponseDto?> ResolveAlertAsync(int id);
}