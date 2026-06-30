using API.DTOs.Donation;
using API.Models;
using API.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace API.Services;

public class DonationService : IDonationService
{
    private readonly LifeFlowDbContext _db;

    public DonationService(LifeFlowDbContext db)
    {
        _db = db;
    }

    public async Task<IEnumerable<DonationResponseDto>> GetAllAsync(
        string? bloodType, string? hospital,
        DateTime? from, DateTime? to,
        int page, int pageSize)
    {
        var query = _db.Donations.AsQueryable();

        if (!string.IsNullOrWhiteSpace(bloodType))
            query = query.Where(d => d.BloodType == bloodType);

        if (!string.IsNullOrWhiteSpace(hospital))
            query = query.Where(d => d.Hospital == hospital);

        if (from.HasValue)
            query = query.Where(d => d.DonationDate >= from.Value);

        if (to.HasValue)
            query = query.Where(d => d.DonationDate <= to.Value);

        return await query
            .OrderByDescending(d => d.DonationDate)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(d => ToDto(d))
            .ToListAsync();
    }

    public async Task<DonationResponseDto?> GetByIdAsync(int id)
    {
        var d = await _db.Donations.FindAsync(id);
        return d == null ? null : ToDto(d);
    }

    public async Task<DonationResponseDto> CreateAsync(CreateDonationDto dto)
    {
        var donation = new Donation
        {
            BloodType = dto.BloodType,
            Units = dto.Units,
            DonationDate = dto.DonationDate,
            DonorName = dto.DonorName,
            Contact = dto.Contact,
            Hospital = dto.Hospital,
            Notes = dto.Notes
        };

        _db.Donations.Add(donation);
        await _db.SaveChangesAsync();

        // Update blood stock
        await UpdateBloodStockAsync(dto.BloodType, dto.Units);

        return ToDto(donation);
    }

    public async Task<DonationResponseDto?> UpdateAsync(int id, CreateDonationDto dto)
    {
        var donation = await _db.Donations.FindAsync(id);
        if (donation == null) return null;

        // Reverse old stock change, apply new
        await UpdateBloodStockAsync(donation.BloodType, -donation.Units);

        donation.BloodType = dto.BloodType;
        donation.Units = dto.Units;
        donation.DonationDate = dto.DonationDate;
        donation.DonorName = dto.DonorName;
        donation.Contact = dto.Contact;
        donation.Hospital = dto.Hospital;
        donation.Notes = dto.Notes;

        await _db.SaveChangesAsync();
        await UpdateBloodStockAsync(dto.BloodType, dto.Units);

        return ToDto(donation);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var donation = await _db.Donations.FindAsync(id);
        if (donation == null) return false;

        await UpdateBloodStockAsync(donation.BloodType, -donation.Units);
        _db.Donations.Remove(donation);
        await _db.SaveChangesAsync();
        return true;
    }

    public async Task<int> GetTotalCountAsync(string? bloodType, string? hospital)
    {
        var query = _db.Donations.AsQueryable();

        if (!string.IsNullOrWhiteSpace(bloodType))
            query = query.Where(d => d.BloodType == bloodType);

        if (!string.IsNullOrWhiteSpace(hospital))
            query = query.Where(d => d.Hospital == hospital);

        return await query.CountAsync();
    }

    // ── Helpers ──────────────────────────────────────────────────────────────

    private async Task UpdateBloodStockAsync(string bloodType, int unitsDelta)
    {
        var stock = await _db.BloodStocks.FindAsync(bloodType);
        if (stock != null)
        {
            stock.AvailableUnits = Math.Max(0, stock.AvailableUnits + unitsDelta);
            stock.Status = stock.AvailableUnits switch
            {
                <= 5 => "Critical",
                <= 15 => "Low",
                <= 30 => "Moderate",
                _ => "Normal"
            };
            await _db.SaveChangesAsync();
        }
    }

    private static DonationResponseDto ToDto(Donation d) => new()
    {
        Id = d.Id,
        BloodType = d.BloodType,
        Units = d.Units,
        DonationDate = d.DonationDate,
        DonorName = d.DonorName,
        Contact = d.Contact,
        Hospital = d.Hospital,
        Notes = d.Notes
    };
}