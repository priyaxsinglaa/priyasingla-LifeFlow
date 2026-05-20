using API.Models;
namespace API.Interfaces;

public interface IUserRepository
{
    Task<User?> GetByEmail(string email);
    Task<User?> GetByPhoneNumber(string phoneNumber);
    Task AddUser(User user);
    Task<User?> GetByEmailOrPhoneNumber(string email, string phone);
}