using API.DTOs.BloodStock;

namespace API.Services.Interfaces;

public interface IBloodStockService
{
    Task<IEnumerable<BloodStockDto>> GetAllAsync();
    Task<BloodStockDto?> GetByBloodTypeAsync(string bloodType);
    Task<BloodStockDto> UpdateAsync(string bloodType, UpdateBloodStockDto dto);
    Task<IEnumerable<string>> GetBloodTypeNamesAsync();
}