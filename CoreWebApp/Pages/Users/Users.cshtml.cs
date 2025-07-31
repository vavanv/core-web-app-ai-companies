using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using CoreWebApp.Services;
using CoreWebApp.Models;
using System.Text.Json;

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

        public async Task<IActionResult> OnGetGetUserAsync(int id)
        {
            try
            {
                var user = await _userService.GetUserByIdAsync(id);
                if (user == null)
                {
                    return new JsonResult(new { success = false, message = "User not found" });
                }

                // Return user data without password
                var userData = new
                {
                    id = user.Id,
                    firstName = user.FirstName,
                    lastName = user.LastName,
                    email = user.Email,
                    isActive = user.IsActive,
                    createdAt = user.CreatedAt,
                    lastLoginAt = user.LastLoginAt
                };

                return new JsonResult(new { success = true, user = userData });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting user details for ID: {UserId}", id);
                return new JsonResult(new { success = false, message = "Error retrieving user details" });
            }
        }
    }
}