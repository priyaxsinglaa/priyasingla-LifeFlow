using API.Interfaces;

namespace API.Services;
using API.DTOs;
using API.Models;
using API.Repositories;
using System.Text.Json;
public class AuthService(IUserRepository repo, IConfiguration config, IJwtService jwtService)
    : IAuthService
{
    private readonly IUserRepository _repo = repo;
    private readonly IConfiguration _config = config;
    private readonly IJwtService _jwtService = jwtService;

    public async Task<string> Register(RegisterDto dto)
    {
        var existingUser = await _repo.GetByEmail(dto.Email);
        var existingPhoneNumber = await _repo.GetByPhoneNumber(dto.PhoneNumber);
        if(existingPhoneNumber != null || existingUser != null)
        {
            return "Account is registered already";
        }

        var hashedPassword = BCrypt.Net.BCrypt.HashPassword(dto.Password);

        var user = new User
        {
            FullName = dto.FullName,
            Email = dto.Email,
            PhoneNumber = dto.PhoneNumber,
            PasswordHash = hashedPassword,
            BloodGroup = dto.BloodGroup,
            Disease = dto.Disease,
            LastDonationDate = dto.LastDonationDate,
            CreatedAt = DateTime.Now,
            Role = dto.Role ?? "Client"
        };

        await _repo.AddUser(user);

        return "Registered Successfully";
    }

    public async Task<string> Login(LoginDto dto)
    {   
        if (string.IsNullOrEmpty(dto.Email) && string.IsNullOrEmpty(dto.PhoneNumber))
            return "Enter your existing Email or Contact Number";

        var user = await _repo.GetByEmailOrPhoneNumber(dto.Email, dto.PhoneNumber); 

        if (user == null)
            return "Account does not exist";

        bool isValid = BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash);

        if (!isValid)
            return "Password is incorrect";
        var token = _jwtService.GenerateToken(user);
        return JsonSerializer.Serialize(new
        {
            Token = token,
            user.FullName,
            user.Email,
            user.Role
        });
    }
}
