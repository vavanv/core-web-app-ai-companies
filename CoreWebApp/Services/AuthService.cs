using CoreWebApp.Models;

namespace CoreWebApp.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserService _userService;
        private readonly ILogger<AuthService> _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AuthService(IUserService userService, ILogger<AuthService> logger, IHttpContextAccessor httpContextAccessor)
        {
            _userService = userService;
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<AuthResult> LoginAsync(string email, string password)
        {
            try
            {
                var user = await _userService.AuthenticateUserAsync(email, password);

                if (user != null)
                {
                    // Store user in session
                    var httpContext = _httpContextAccessor.HttpContext;
                    if (httpContext != null)
                    {
                        httpContext.Session.SetString("UserId", user.Id.ToString());
                        httpContext.Session.SetString("UserEmail", user.Email);
                        httpContext.Session.SetString("UserName", user.FullName);
                    }

                    _logger.LogInformation("User logged in successfully: {Email}", email);

                    return new AuthResult
                    {
                        Success = true,
                        Message = "Login successful",
                        User = user
                    };
                }

                return new AuthResult
                {
                    Success = false,
                    Message = "Invalid email or password"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during login for email: {Email}", email);
                return new AuthResult
                {
                    Success = false,
                    Message = "An error occurred during login"
                };
            }
        }

        public Task<bool> LogoutAsync()
        {
            try
            {
                // Clear session
                var httpContext = _httpContextAccessor.HttpContext;
                if (httpContext != null)
                {
                    httpContext.Session.Clear();
                }

                _logger.LogInformation("User logged out successfully");
                return Task.FromResult(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during logout");
                return Task.FromResult(false);
            }
        }

        public async Task<AuthResult> RegisterAsync(User user, string password)
        {
            try
            {
                // Set the password hash
                user.PasswordHash = password; // The UserService will hash this

                var success = await _userService.CreateUserAsync(user);

                if (success)
                {
                    _logger.LogInformation("User registered successfully: {Email}", user.Email);
                    return new AuthResult
                    {
                        Success = true,
                        Message = "Registration successful",
                        User = user
                    };
                }

                return new AuthResult
                {
                    Success = false,
                    Message = "User with this email already exists"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during registration for email: {Email}", user.Email);
                return new AuthResult
                {
                    Success = false,
                    Message = "An error occurred during registration"
                };
            }
        }

        public Task<bool> IsUserAuthenticatedAsync()
        {
            var httpContext = _httpContextAccessor.HttpContext;
            if (httpContext == null) return Task.FromResult(false);

            var userId = httpContext.Session.GetString("UserId");
            return Task.FromResult(!string.IsNullOrEmpty(userId));
        }

        public async Task<User?> GetCurrentUserAsync()
        {
            var httpContext = _httpContextAccessor.HttpContext;
            if (httpContext == null) return null;

            var userId = httpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(userId)) return null;

            if (int.TryParse(userId, out var id))
            {
                return await _userService.GetUserByIdAsync(id);
            }

            return null;
        }
    }
}