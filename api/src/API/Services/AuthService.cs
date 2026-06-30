using API.DTOs.Auth;
using API.Helpers;
using API.Models;
using API.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace API.Services;

public class AuthService : IAuthService
{
    private readonly LifeFlowDbContext _db;
    private readonly JwtHelper _jwt;
    private readonly IConfiguration _config;

    public AuthService(LifeFlowDbContext db, JwtHelper jwt, IConfiguration config)
    {
        _db = db;
        _jwt = jwt;
        _config = config;
    }

    public async Task<AuthResponseDto> RegisterAsync(RegisterDto dto)
    {
        // Check for duplicates
        if (await _db.Users.AnyAsync(u => u.Username == dto.Username))
            throw new InvalidOperationException("Username already taken.");

        if (await _db.Users.AnyAsync(u => u.Email == dto.Email))
            throw new InvalidOperationException("Email already registered.");

        if (await _db.Users.AnyAsync(u => u.PhoneNumber == dto.PhoneNumber))
            throw new InvalidOperationException("Phone number already registered.");

        var user = new User
        {
            Username = dto.Username,
            Email = dto.Email,
            PhoneNumber = dto.PhoneNumber,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
            Role = dto.Role,
            CreatedAt = DateTime.UtcNow
        };

        _db.Users.Add(user);
        await _db.SaveChangesAsync();

        return BuildResponse(user);
    }

    public async Task<AuthResponseDto> LoginAsync(LoginDto dto)
    {
        var user = await _db.Users
            .FirstOrDefaultAsync(u => u.Email == dto.Email);

        if (user == null || !BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
            throw new UnauthorizedAccessException("Invalid email or password.");

        return BuildResponse(user);
    }

    public async Task<AuthResponseDto> GuestLoginAsync()
    {
        // Return a short-lived guest token without hitting DB
        var guestUser = new User
        {
            Id = 0,
            Username = "guest",
            Email = "guest@lifeflow.com",
            PhoneNumber = "0000000000",
            PasswordHash = "",
            Role = "Guest"
        };

        return await Task.FromResult(BuildResponse(guestUser));
    }

    private AuthResponseDto BuildResponse(User user)
    {
        var token = _jwt.GenerateToken(user);
        var expiry = double.Parse(_config["Jwt:ExpiryMinutes"] ?? "60");

        return new AuthResponseDto
        {
            Token = token,
            Username = user.Username,
            Email = user.Email,
            Role = user.Role,
            ExpiresAt = DateTime.Now.AddMinutes(expiry)
        };
    }
}