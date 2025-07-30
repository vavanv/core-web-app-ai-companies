using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using CoreWebApp.Services;
using CoreWebApp.Models;

namespace CoreWebApp.Pages
{
    public class UsersModel : PageModel
    {
        private readonly ILogger<UsersModel> _logger;
        private readonly IUserService _userService;

        public UsersModel(ILogger<UsersModel> logger, IUserService userService)
        {
            _logger = logger;
            _userService = userService;
        }

        public IEnumerable<User>? Users { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            try
            {
                Users = await _userService.GetAllUsersAsync();
                return Page();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading users");
                return RedirectToPage("/Error");
            }
        }
    }
}