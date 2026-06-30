namespace API.DTOs.Donation;

public class DonationResponseDto
{
    public int Id { get; set; }
    public string BloodType { get; set; } = null!;
    public int Units { get; set; }
    public DateTime DonationDate { get; set; }
    public string? DonorName { get; set; }
    public string? Contact { get; set; }
    public string Hospital { get; set; } = null!;
    public string? Notes { get; set; }
}