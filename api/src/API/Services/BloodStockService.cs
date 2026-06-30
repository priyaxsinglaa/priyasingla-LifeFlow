using API.DTOs.BloodStock;
using API.Models;
using API.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace API.Services;

public class BloodStockService : IBloodStockService
{
    private readonly LifeFlowDbContext _db;

    public BloodStockService(LifeFlowDbContext db)
    {
        _db = db;
    }

    public async Task<IEnumerable<BloodStockDto>> GetAllAsync()
    {
        //return await _db.BloodStocks
        //    .Select(s => ToDto(s))
        //    .ToListAsync();
        var stocks = await _db.BloodStocks.ToListAsync();

        // 2. Now map them to your DTO objects in memory safely
        return stocks.Select(s => ToDto(s));
    }

    public async Task<BloodStockDto?> GetByBloodTypeAsync(string bloodType)
    {
        var stock = await _db.BloodStocks.FindAsync(bloodType);
        return stock == null ? null : ToDto(stock);
    }

    public async Task<BloodStockDto> UpdateAsync(string bloodType, UpdateBloodStockDto dto)
    {
        var stock = await _db.BloodStocks.FindAsync(bloodType)
            ?? throw new KeyNotFoundException($"Blood type {bloodType} not found.");

        stock.AvailableUnits = dto.AvailableUnits;
        stock.PredictedDemand = dto.PredictedDemand;
        stock.Status = dto.AvailableUnits switch
        {
            <= 5 => "Critical",
            <= 15 => "Low",
            <= 30 => "Moderate",
            _ => "Normal"
        };

        await _db.SaveChangesAsync();
        return ToDto(stock);
    }

    public async Task<IEnumerable<string>> GetBloodTypeNamesAsync()
    {
        return await _db.BloodStocks
            .Select(s => s.BloodType)
            .ToListAsync();
    }

    private static BloodStockDto ToDto(BloodStock s) => new()
    {
        BloodType = s.BloodType,
        AvailableUnits = s.AvailableUnits,
        PredictedDemand = s.PredictedDemand,
        SupplyLevel = s.SupplyLevel,
        Status = s.Status
    };
}