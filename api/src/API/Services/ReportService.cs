using API.DTOs.Alert;
using API.DTOs.Donation;
using API.DTOs.Report;
using API.Services.Interfaces;
using ClosedXML.Excel;
using Microsoft.EntityFrameworkCore;
using API.Models;

namespace API.Services;

public class ReportService : IReportService
{
    private readonly LifeFlowDbContext _db;

    public ReportService(LifeFlowDbContext db)
    {
        _db = db;
    }

    // ── GET /api/reports ─────────────────────────────────────────────────────

    public async Task<ReportResponseDto> GenerateReportAsync(
        DateTime from, DateTime to, string? hospital = null)
    {
        // Base donation query — filter by date range
        var donationQuery = _db.Donations
            .Where(d => d.DonationDate >= from && d.DonationDate <= to);

        // Apply hospital filter only if provided (not null/empty = all hospitals)
        if (!string.IsNullOrWhiteSpace(hospital))
            donationQuery = donationQuery.Where(d => d.Hospital == hospital);

        var donations = await donationQuery
            .OrderByDescending(d => d.DonationDate)
            .ToListAsync();

        // Same hospital filter applied to alerts
        var alertQuery = _db.ShortageAlerts
            .Where(a => a.CreatedDate >= from && a.CreatedDate <= to);

        if (!string.IsNullOrWhiteSpace(hospital))
            alertQuery = alertQuery.Where(a => a.Hospital == hospital);

        var alerts = await alertQuery.ToListAsync();

        return new ReportResponseDto
        {
            FromDate = from,
            ToDate = to,
            TotalDonations = donations.Count,
            TotalUnits = donations.Sum(d => d.Units),

            // Map to DTOs — never expose raw EF entities to the API response
            Donations = donations.Select(d => new DonationResponseDto
            {
                Id = d.Id,
                BloodType = d.BloodType,
                Units = d.Units,
                DonationDate = d.DonationDate,
                DonorName = d.DonorName,
                Contact = d.Contact,
                Hospital = d.Hospital,
                Notes = d.Notes
            }).ToList(),

            Alerts = alerts.Select(a => new AlertResponseDto
            {
                Id = a.Id,
                Severity = a.Severity,
                BloodType = a.BloodType,
                Hospital = a.Hospital,
                Units = a.Units,
                IsActive = a.IsActive,
                CreatedDate = a.CreatedDate
            }).ToList(),

            // Breakdown dictionaries — used by the chart/summary cards in your UI
            UnitsByBloodType = donations
                .GroupBy(d => d.BloodType)
                .ToDictionary(g => g.Key, g => g.Sum(d => d.Units)),

            DonationsByHospital = donations
                .GroupBy(d => d.Hospital)
                .ToDictionary(g => g.Key, g => g.Count())
        };
    }

    // ── GET /api/reports/export ──────────────────────────────────────────────

    public async Task<byte[]> ExportToExcelAsync(
        DateTime from, DateTime to, string? hospital = null)
    {
        // Reuse GenerateReportAsync so logic is never duplicated
        var report = await GenerateReportAsync(from, to, hospital);

        using var workbook = new XLWorkbook();

        // ── Sheet 1: Donations ────────────────────────────────────────────
        var ws1 = workbook.Worksheets.Add("Donations");

        // Headers
        string[] donationHeaders = ["ID", "Blood Type", "Units", "Donor Name", "Hospital", "Donation Date", "Contact"];
        for (int col = 0; col < donationHeaders.Length; col++)
            ws1.Cell(1, col + 1).Value = donationHeaders[col];

        StyleHeaderRow(ws1.Row(1));

        int row = 2;
        foreach (var d in report.Donations)
        {
            ws1.Cell(row, 1).Value = d.Id;
            ws1.Cell(row, 2).Value = d.BloodType;
            ws1.Cell(row, 3).Value = d.Units;
            ws1.Cell(row, 4).Value = d.DonorName ?? "-";
            ws1.Cell(row, 5).Value = d.Hospital;
            ws1.Cell(row, 6).Value = d.DonationDate.ToString("yyyy-MM-dd");
            ws1.Cell(row, 7).Value = d.Contact ?? "-";

            // Zebra striping for readability
            if (row % 2 == 0)
                ws1.Row(row).Style.Fill.BackgroundColor = XLColor.FromHtml("#FFF5F5");

            row++;
        }
        ws1.Columns().AdjustToContents();

        // ── Sheet 2: Alerts ───────────────────────────────────────────────
        var ws2 = workbook.Worksheets.Add("Alerts");

        string[] alertHeaders = ["ID", "Blood Type", "Hospital", "Severity", "Units", "Status", "Created Date"];
        for (int col = 0; col < alertHeaders.Length; col++)
            ws2.Cell(1, col + 1).Value = alertHeaders[col];

        StyleHeaderRow(ws2.Row(1));

        row = 2;
        foreach (var a in report.Alerts)
        {
            ws2.Cell(row, 1).Value = a.Id;
            ws2.Cell(row, 2).Value = a.BloodType;
            ws2.Cell(row, 3).Value = a.Hospital;
            ws2.Cell(row, 4).Value = a.Severity;
            ws2.Cell(row, 5).Value = a.Units;
            ws2.Cell(row, 6).Value = a.IsActive ? "Active" : "Resolved";
            ws2.Cell(row, 7).Value = a.CreatedDate.ToString("yyyy-MM-dd");

            // Colour-code severity in the Severity column
            var severityCell = ws2.Cell(row, 4);
            severityCell.Style.Font.FontColor = a.Severity switch
            {
                "Critical" => XLColor.FromHtml("#9B2335"),
                "High" => XLColor.FromHtml("#C05621"),
                "Medium" => XLColor.FromHtml("#B7791F"),
                _ => XLColor.FromHtml("#2B6CB0")
            };
            severityCell.Style.Font.Bold = true;

            if (row % 2 == 0)
                ws2.Row(row).Style.Fill.BackgroundColor = XLColor.FromHtml("#FFF5F5");

            row++;
        }
        ws2.Columns().AdjustToContents();

        // ── Sheet 3: Summary ──────────────────────────────────────────────
        var ws3 = workbook.Worksheets.Add("Summary");

        // Top summary block
        ws3.Cell(1, 1).Value = "Metric";
        ws3.Cell(1, 2).Value = "Value";
        StyleHeaderRow(ws3.Row(1));

        ws3.Cell(2, 1).Value = "Report Period";
        ws3.Cell(2, 2).Value = $"{from:yyyy-MM-dd} to {to:yyyy-MM-dd}";
        ws3.Cell(3, 1).Value = "Hospital Filter";
        ws3.Cell(3, 2).Value = string.IsNullOrWhiteSpace(hospital) ? "All Hospitals" : hospital;
        ws3.Cell(4, 1).Value = "Total Donations";
        ws3.Cell(4, 2).Value = report.TotalDonations;
        ws3.Cell(5, 1).Value = "Total Units";
        ws3.Cell(5, 2).Value = report.TotalUnits;
        ws3.Cell(6, 1).Value = "Total Alerts";
        ws3.Cell(6, 2).Value = report.Alerts.Count;
        ws3.Cell(7, 1).Value = "Active Alerts";
        ws3.Cell(7, 2).Value = report.Alerts.Count(a => a.IsActive);

        // Units by blood type breakdown
        ws3.Cell(9, 1).Value = "Blood Type";
        ws3.Cell(9, 2).Value = "Units Donated";
        StyleHeaderRow(ws3.Row(9));

        int summaryRow = 10;
        foreach (var kvp in report.UnitsByBloodType.OrderByDescending(x => x.Value))
        {
            ws3.Cell(summaryRow, 1).Value = kvp.Key;
            ws3.Cell(summaryRow, 2).Value = kvp.Value;
            summaryRow++;
        }

        // Donations by hospital breakdown
        summaryRow++;
        ws3.Cell(summaryRow, 1).Value = "Hospital";
        ws3.Cell(summaryRow, 2).Value = "Donation Count";
        StyleHeaderRow(ws3.Row(summaryRow));
        summaryRow++;

        foreach (var kvp in report.DonationsByHospital.OrderByDescending(x => x.Value))
        {
            ws3.Cell(summaryRow, 1).Value = kvp.Key;
            ws3.Cell(summaryRow, 2).Value = kvp.Value;
            summaryRow++;
        }

        ws3.Columns().AdjustToContents();

        using var stream = new MemoryStream();
        workbook.SaveAs(stream);
        return stream.ToArray();
    }

    // ── GET /api/reports/hospitals ───────────────────────────────────────────
    // Returns distinct hospital names from Donations table for the UI dropdown

    public async Task<IEnumerable<string>> GetHospitalNamesAsync()
    {
        return await _db.Donations
            .Select(d => d.Hospital)
            .Distinct()
            .OrderBy(h => h)
            .ToListAsync();
    }

    // ── Helper ───────────────────────────────────────────────────────────────

    private static void StyleHeaderRow(IXLRow row)
    {
        row.Style.Font.Bold = true;
        row.Style.Fill.BackgroundColor = XLColor.FromHtml("#C00000"); // dark red matching your theme
        row.Style.Font.FontColor = XLColor.White;
        row.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
    }
}