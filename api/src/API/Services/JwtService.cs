using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using API.Interfaces;
using Microsoft.IdentityModel.Tokens;
using API.Models;

namespace API.Services;

public class JwtService(IConfiguration config): IJwtService
{
    private readonly IConfiguration _config = config;
    public string GenerateToken(User user)
    {
        var claims = new[]
        {
            new Claim(ClaimTypes.Name, user.Email),
            new Claim(ClaimTypes.Role, user.Role ?? "Client"),
            new Claim("UserId", user.Id.ToString())
        };

        var jwtKey = _config["Jwt:Key"] ?? throw new InvalidOperationException("JWT key is not configured.");
        var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(jwtKey)
        );

        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _config["Jwt:Issuer"],
            audience: _config["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddHours(2),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
