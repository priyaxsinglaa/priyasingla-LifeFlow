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
        return await _context.Users
            .FirstOrDefaultAsync(u => (!string.IsNullOrEmpty(email) && u.Email == email) || 
                                      (!string.IsNullOrEmpty(phoneNumber) && u.PhoneNumber == phoneNumber));
    }
}