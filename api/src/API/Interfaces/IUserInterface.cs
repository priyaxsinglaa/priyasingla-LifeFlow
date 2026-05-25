using API.DTOs;
namespace API.Interfaces;

public interface IUserInterface
{
    Task<string> SaveOnboarding(int userId, OnBoardingDto dto);
}