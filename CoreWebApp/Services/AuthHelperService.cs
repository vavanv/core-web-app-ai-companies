using CoreWebApp.Models;

namespace CoreWebApp.Services
{
    public class AuthHelperService : IAuthHelperService
    {
        private readonly IAuthService _authService;

        public AuthHelperService(IAuthService authService)
        {
            _authService = authService;
        }

        public Task<bool> IsUserAuthenticatedAsync()
        {
            return _authService.IsUserAuthenticatedAsync();
        }

        public Task<User?> GetCurrentUserAsync()
        {
            return _authService.GetCurrentUserAsync();
        }
    }
} 