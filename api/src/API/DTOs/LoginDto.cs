namespace API.DTOs;

public class LoginDto
{
    public string? Email { get; set; }
    public string? PhoneNumber { get; set; }
    public required string Password { get; set; }
}
