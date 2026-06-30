using API.DTOs.Alert;
using API.Models;
using API.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace API.Services;

public class AlertService : IAlertService
{
    private readonly LifeFlowDbContext _db;

    // Severity thresholds (units)
    private const int CriticalThreshold = 5;
    private const int HighThreshold = 10;
    private const int MediumThreshold = 20;
    private const int LowThreshold = 30;

    public AlertService(LifeFlowDbContext db)
    {
        _db = db;
    }

    public async Task<IEnumerable<AlertResponseDto>> GetActiveAlertsAsync()
    {
        return await _db.ShortageAlerts
            .Where(a => a.IsActive)
            .OrderByDescending(a => a.CreatedDate)
            .Select(a => ToDto(a))
            .ToListAsync();
    }

    public async Task<IEnumerable<AlertResponseDto>> EvaluateAndCreateAlertsAsync()
    {
        var stocks = await _db.BloodStocks.ToListAsync();
        var newAlerts = new List<ShortageAlert>();

        foreach (var stock in stocks)
        {
            var severity = GetSeverity(stock.AvailableUnits);
            if (severity == null) continue; // Stock is fine

            // Check if an unresolved alert already exists for this blood type
            bool exists = await _db.ShortageAlerts.AnyAsync(a =>
                a.BloodType == stock.BloodType &&
                a.IsActive);

            if (!exists)
            {
                var alert = new ShortageAlert
                {
                    BloodType = stock.BloodType,
                    Hospital = "General", // Can be extended per-hospital
                    Units = stock.AvailableUnits,
                    Severity = severity,
                    IsActive = true,
                    CreatedDate = DateTime.UtcNow
                };
                newAlerts.Add(alert);
            }
        }

        if (newAlerts.Any())
        {
            _db.ShortageAlerts.AddRange(newAlerts);
            await _db.SaveChangesAsync();
        }

        return newAlerts.Select(a => ToDto(a));
    }

    public async Task<AlertResponseDto?> ResolveAlertAsync(int id)
    {
        var alert = await _db.ShortageAlerts.FindAsync(id);
        if (alert == null) return null;

        alert.IsActive = false;
        await _db.SaveChangesAsync();
        return ToDto(alert);
    }

    // ── Helpers ──────────────────────────────────────────────────────────────

    private static string? GetSeverity(int units) => units switch
    {
        <= CriticalThreshold => "Critical",
        <= HighThreshold => "High",
        <= MediumThreshold => "Medium",
        <= LowThreshold => "Low",
        _ => null
    };

    private static AlertResponseDto ToDto(ShortageAlert a) => new()
    {
        Id = a.Id,
        Severity = a.Severity,
        BloodType = a.BloodType,
        Hospital = a.Hospital,
        Units = a.Units,
        IsActive = a.IsActive,
        CreatedDate = a.CreatedDate
    };
}