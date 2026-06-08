namespace API.DTOs;

public class LoginDto
{

    public  required string Email { get; set; } = string.Empty;
    public required string Password { get; set; } = string.Empty;
}
public class RegisterDto
{
    public required string Username { get; set; }= string.Empty;
    public required string Email { get; set; } = string.Empty;
    public required string PhoneNumber { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}
public class AuthResponseDto
{
    public string Token { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public DateTime ExpiresAt { get; set; }
}