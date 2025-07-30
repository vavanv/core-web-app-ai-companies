using CoreWebApp.Models;

namespace CoreWebApp.Services
{
    public interface IAuthHelperService
    {
        Task<bool> IsUserAuthenticatedAsync();
        Task<User?> GetCurrentUserAsync();
    }
} 