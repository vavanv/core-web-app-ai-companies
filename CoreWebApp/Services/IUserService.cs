using CoreWebApp.Models;

namespace CoreWebApp.Services
{
    public interface IUserService
    {
            Task<User?> AuthenticateUserAsync(string email, string password);
    Task<User?> GetUserByEmailAsync(string email);
    Task<bool> CreateUserAsync(User user);
    Task<IEnumerable<User>> GetAllUsersAsync();
    Task<User?> GetUserByIdAsync(int id);
    }
}