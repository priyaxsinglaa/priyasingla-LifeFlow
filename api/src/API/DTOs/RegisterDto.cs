using System.ComponentModel.DataAnnotations;
namespace API.DTOs;

public class RegisterDto
{
    public required string FullName { get; set; }

    [EmailAddress(ErrorMessage = "Invalid email format")]
    public required string Email { get; set; }
    [Phone(ErrorMessage = "Invalid phone number")]
    [RegularExpression(@"^\d{10}$", ErrorMessage = "Phone number must be 10 digits")] 
    public required string PhoneNumber { get; set; }
    [StringLength(20, MinimumLength = 6, ErrorMessage = "Password must be 6-20 characters")]
    public required string Password { get; set; }
    [RegularExpression(@"^(A|B|AB|O)[+-]$", ErrorMessage = "Invalid Blood Group")]
    public required string BloodGroup { get; set; }
    public string? Disease { get; set; } 
    public DateTime? LastDonationDate { get; set; }
}
