using API.Interfaces;
using API.DTOs;
using API.Models;
namespace API.Services;

public class UserService(IUserRepository repo) : IUserInterface
{
        private readonly IUserRepository _repo = repo;
        public async Task<string> SaveOnboarding(int userId, OnBoardingDto dto)
        {
            var health = new HealthProfile
            {
                UserId = userId,
                Weight = dto.Weight,
                BloodGroup = dto.BloodGroup,
                RhFactor = dto.RhFactor,
                Diseases = dto.Diseases,
                CreatedAt = DateTime.UtcNow
            };

            await _repo.AddHealthProfile(health);

            return "Onboarding saved successfully";
        }

        
}
