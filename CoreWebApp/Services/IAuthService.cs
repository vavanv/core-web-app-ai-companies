using CoreWebApp.Models;

namespace CoreWebApp.Services
{
    public interface IAuthService
    {
            Task<AuthResult> LoginAsync(string email, string password);
    Task<bool> LogoutAsync();
    Task<bool> IsUserAuthenticatedAsync();
    Task<User?> GetCurrentUserAsync();
    }

    public class AuthResult
    {
        public bool Success { get; set; }
        public string? Message { get; set; }
        public User? User { get; set; }
        public string? Token { get; set; }
    }
}