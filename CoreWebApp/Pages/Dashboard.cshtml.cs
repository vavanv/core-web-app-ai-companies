using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using CoreWebApp.Services;
using CoreWebApp.Models;

namespace CoreWebApp.Pages
{
    public class DashboardModel : PageModel
    {
        private readonly ILogger<DashboardModel> _logger;
        private readonly IAuthService _authService;
        private readonly IUserService _userService;

        public DashboardModel(ILogger<DashboardModel> logger, IAuthService authService, IUserService userService)
        {
            _logger = logger;
            _authService = authService;
            _userService = userService;
        }

        public User? CurrentUser { get; set; }
        public IEnumerable<User>? AllUsers { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            try
            {
                // Get current user (in a real app, this would come from authentication context)
                CurrentUser = await _authService.GetCurrentUserAsync();

                // Get all users for demonstration
                AllUsers = await _userService.GetAllUsersAsync();

                return Page();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading dashboard");
                return RedirectToPage("/Error");
            }
        }
    }
}