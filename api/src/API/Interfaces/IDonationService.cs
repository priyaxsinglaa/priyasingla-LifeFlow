using API.DTOs;
namespace API.Interfaces;

public interface IDonationService
{
    Task<string> AddDonation(int userId, DonationDto dto);
    Task<int> GetTotalDonationCount(int userId);
}