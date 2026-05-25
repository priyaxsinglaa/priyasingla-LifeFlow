using API.Interfaces;
using API.Models;
using Microsoft.EntityFrameworkCore;
namespace API.Repositories;
public class UserRepository(LifeFlowDbContext context) : IUserRepository
{
    private readonly LifeFlowDbContext _context = context;
    
    public async Task<User?> GetByEmail(string email)
    {
        return await _context.Users
            .FirstOrDefaultAsync(u => u.Email == email);
    }
    public async Task<User?> GetByPhoneNumber(string phoneNumber)
    {
        return await _context.Users
            .FirstOrDefaultAsync(u=> u.PhoneNumber == phoneNumber);
    }
    public async Task AddUser(User user)
    {
        await _context.Users.AddAsync(user);
        await _context.SaveChangesAsync();
    }
    public async Task<User?> GetByEmailOrPhoneNumber(string? email ,string? phoneNumber)
    {
        email = string.IsNullOrWhiteSpace(email) ? null : email.Trim();
        phoneNumber = string.IsNullOrWhiteSpace(phoneNumber) ? null : phoneNumber.Trim();
        if (email == null && phoneNumber == null)
        {
            return null;
        }
        return await _context.Users
            .FirstOrDefaultAsync(u => (!string.IsNullOrEmpty(email) && u.Email == email) || 
                                      (!string.IsNullOrEmpty(phoneNumber) && u.PhoneNumber == phoneNumber));
    }

    public async Task AddHealthProfile(HealthProfile health)
    {
            await _context.HealthProfiles.AddAsync(health);
            await _context.SaveChangesAsync();
        
    }
    public async Task AddDonation(Donation donation)
    {
        await _context.Donations.AddAsync(donation);
        await _context.SaveChangesAsync();
    }
}