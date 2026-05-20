using System.ComponentModel.DataAnnotations;
namespace API.DTOs;

public class LoginDto
{
    [EmailAddress(ErrorMessage = "Invalid email format")]
    public string? Email { get; set; }
    [Phone(ErrorMessage = "Invalid phone number")]
    public string? PhoneNumber { get; set; }
    [StringLength(20, MinimumLength = 6, ErrorMessage = "Password must be 6-20 characters")]
    public required string Password { get; set; }
}
