namespace API.Services;
using API.DTOs;
using API.Models;
using API.Repositories;
public class AuthService
{
    private readonly UserRepository _repo;

    public AuthService(UserRepository repo)
    {
        _repo = repo;
    }

    public async Task<string> Register(RegisterDto dto)
    {
        var existingUser = await _repo.GetByEmail(dto.Email);
        var existingPhoneNumbser = await _repo.GetByPhoneNumber(dto.PhoneNumber);
        if(existingPhoneNumbser != null || existingUser != null)
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
            CreatedAt = DateTime.Now
        };

        await _repo.AddUser(user);

        return "Registered Successfully";
    }

    public async Task<string> Login(LoginDto dto)
    {   
        if (string.IsNullOrEmpty(dto.Email) && string.IsNullOrEmpty(dto.PhoneNumber))
        return "Enter your existing Email or Contact Number";

        var user = await _repo.GetByEmailOrPhoneNumber(dto.Email, dto.PhoneNumber); 
        //var user = await _repo.GetByEmail(dto.Email);

        if (user == null)
            return "Account does not exist";

        bool isValid = BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash);

        if (!isValid)
            return "Password is incorrect";

        return "Logged in successfully";
    }
}
