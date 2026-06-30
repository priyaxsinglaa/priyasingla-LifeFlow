using API.DTOs.Donation;
using API.DTOs.Alert;

namespace API.DTOs.Report;

public class ReportResponseDto
{
    public DateTime GeneratedAt { get; set; } = DateTime.UtcNow;
    public DateTime FromDate { get; set; }
    public DateTime ToDate { get; set; }
    public int TotalDonations { get; set; }
    public int TotalUnits { get; set; }
    public List<DonationResponseDto> Donations { get; set; } = new();
    public List<AlertResponseDto> Alerts { get; set; } = new();
    public Dictionary<string, int> UnitsByBloodType { get; set; } = new();
    public Dictionary<string, int> DonationsByHospital { get; set; } = new();
}