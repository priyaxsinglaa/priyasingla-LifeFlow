using API.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("api/reports")]
public class ReportsController : ControllerBase
{
    private readonly IReportService _reportService;

    public ReportsController(IReportService reportService)
    {
        _reportService = reportService;
    }

    // GET api/reports?from=2024-01-01&to=2024-12-31&hospital=City Hospital
    // hospital param is optional — null means "all hospitals" (matches your UI dropdown)
    [HttpGet]
    public async Task<IActionResult> GetReport(
        [FromQuery] DateTime? from,
        [FromQuery] DateTime? to,
        [FromQuery] string? hospital)
    {
        if (from.HasValue && to.HasValue && from > to)
            return BadRequest(new { message = "From date cannot be after To date." });

        var fromDate = from ?? DateTime.Today.AddMonths(-1);
        var toDate = to ?? DateTime.Today;

        var report = await _reportService.GenerateReportAsync(fromDate, toDate, hospital);
        return Ok(report);
    }

    // GET api/reports/export?from=2024-01-01&to=2024-12-31&hospital=City Hospital
    [HttpGet("export")]
    public async Task<IActionResult> ExportExcel(
        [FromQuery] DateTime? from,
        [FromQuery] DateTime? to,
        [FromQuery] string? hospital)
    {
        if (from.HasValue && to.HasValue && from > to)
            return BadRequest(new { message = "From date cannot be after To date." });

        var fromDate = from ?? DateTime.Today.AddMonths(-1);
        var toDate = to ?? DateTime.Today;

        var excelBytes = await _reportService.ExportToExcelAsync(fromDate, toDate, hospital);

        // File name includes hospital if filtered, e.g. LifeFlow_Report_CityHospital_20240101_20241231.xlsx
        var hospitalPart = string.IsNullOrWhiteSpace(hospital)
            ? ""
            : $"_{hospital.Replace(" ", "")}";

        return File(
            excelBytes,
            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            $"LifeFlow_Report{hospitalPart}_{fromDate:yyyyMMdd}_{toDate:yyyyMMdd}.xlsx"
        );
    }

    // GET api/reports/hospitals
    // Returns distinct hospital names for the dropdown in your UI
    [HttpGet("hospitals")]
    public async Task<IActionResult> GetHospitals()
    {
        var hospitals = await _reportService.GetHospitalNamesAsync();
        return Ok(hospitals);
    }
}