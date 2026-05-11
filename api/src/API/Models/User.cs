namespace API.Models;

public partial class User
{
    public int Id { get; set; }

    public string FullName { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string PhoneNumber { get; set; } = null!;

    public string PasswordHash { get; set; } = null!;

    public string? BloodGroup { get; set; }

    public string? Disease { get; set; }

    public DateTime? LastDonationDate { get; set; }

    public DateTime? CreatedAt { get; set; }
}
