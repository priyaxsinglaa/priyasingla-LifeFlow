using API.DTOs.Donation;

namespace API.Services.Interfaces;

public interface IDonationService
{
    Task<IEnumerable<DonationResponseDto>> GetAllAsync(
        string? bloodType, string? hospital, DateTime? from, DateTime? to,
        int page, int pageSize);

    Task<DonationResponseDto?> GetByIdAsync(int id);
    Task<DonationResponseDto> CreateAsync(CreateDonationDto dto);
    Task<DonationResponseDto?> UpdateAsync(int id, CreateDonationDto dto);
    Task<bool> DeleteAsync(int id);
    Task<int> GetTotalCountAsync(string? bloodType, string? hospital);
}