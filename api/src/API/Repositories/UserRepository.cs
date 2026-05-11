using API.Models;
using Microsoft.EntityFrameworkCore;
namespace API.Repositories;
public class UserRepository
{
    private readonly LifeFlowDbContext _context;

    public UserRepository(LifeFlowDbContext context)
    {
        _context = context;
    }

    
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
            .FirstOrDefaultAsync(u=>(u.Email != null && u.Email == email) || (u.PhoneNumber != null && u.PhoneNumber == phoneNumber));
    }
}