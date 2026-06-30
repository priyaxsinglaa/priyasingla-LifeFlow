using System.ComponentModel.DataAnnotations;

namespace API.DTOs.Donation;

public class CreateDonationDto
{
    [Required]
    [MaxLength(5)]
    public string BloodType { get; set; } = null!;

    [Required]
    [Range(1, 10)]
    public int Units { get; set; }

    [Required]
    public DateTime DonationDate { get; set; }

    [MaxLength(100)]
    public string? DonorName { get; set; }

    [MaxLength(50)]
    public string? Contact { get; set; }

    [Required]
    [MaxLength(100)]
    public string Hospital { get; set; } = null!;

    public string? Notes { get; set; }
}