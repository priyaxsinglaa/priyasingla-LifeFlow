using API.DTOs.Report;

namespace API.Services.Interfaces;

public interface IReportService
{
    Task<ReportResponseDto> GenerateReportAsync(DateTime from, DateTime to, string? hospital = null);
    Task<byte[]> ExportToExcelAsync(DateTime from, DateTime to, string? hospital = null);
    Task<IEnumerable<string>> GetHospitalNamesAsync();
}