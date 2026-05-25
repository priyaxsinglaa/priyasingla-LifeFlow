using API.DTOs;
using API.Models;
using API.Interfaces;
using Microsoft.EntityFrameworkCore;
namespace API.Services;

public class DonationService(IUserRepository repo, LifeFlowDbContext context): IDonationService 
{
    private readonly IUserRepository _repo =  repo;
    private readonly LifeFlowDbContext _context = context;
    public async Task<string> AddDonation(int userId, DonationDto dto)
    {
        var donation = new Donation
        {
            UserId = userId,
            DonationDate = dto.DonationDate,
            Location = dto.Location,
            Notes = dto.Notes
        };

        await _repo.AddDonation(donation);

        return "Donation recorded successfully";
    }
    public async Task<int> GetTotalDonationCount(int userId)
    {
        
        return await _context.Donations.CountAsync(d => d.UserId == userId);
    }
}