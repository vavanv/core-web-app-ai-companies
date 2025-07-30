using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using CoreWebApp.Services;

namespace CoreWebApp.Pages
{
    public class LoginModel : PageModel
    {
        private readonly ILogger<LoginModel> _logger;
        private readonly IAuthService _authService;

        public LoginModel(ILogger<LoginModel> logger, IAuthService authService)
        {
            _logger = logger;
            _authService = authService;
        }

        [BindProperty]
        public InputModel Input { get; set; } = new InputModel();

        public string? ReturnUrl { get; set; }

        public class InputModel
        {
            [Required(ErrorMessage = "Email is required")]
            [EmailAddress(ErrorMessage = "Invalid email address")]
            public string Email { get; set; } = string.Empty;

            [Required(ErrorMessage = "Password is required")]
            [DataType(DataType.Password)]
            [StringLength(100, ErrorMessage = "Password must be at least {2} characters long.", MinimumLength = 6)]
            public string Password { get; set; } = string.Empty;

            [Display(Name = "Remember me?")]
            public bool RememberMe { get; set; }
        }

        public void OnGet(string? returnUrl = null)
        {
            ReturnUrl = returnUrl;
        }

        public async Task<IActionResult> OnPostAsync(string? returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");

            if (ModelState.IsValid)
            {
                var authResult = await _authService.LoginAsync(Input.Email, Input.Password);

                if (authResult.Success)
                {
                    _logger.LogInformation("User logged in: {Email}", Input.Email);

                    // In a real application, you would:
                    // 1. Create authentication cookies/tokens
                    // 2. Redirect to the intended page

                    return LocalRedirect(returnUrl);
                }
                else
                {
                    ModelState.AddModelError(string.Empty, authResult.Message ?? "Invalid login attempt.");
                    return Page();
                }
            }

            // If we got this far, something failed, redisplay form
            return Page();
        }
    }
}