using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using CoreWebApp.Data;
using CoreWebApp.Models;
using CoreWebApp.Services;
using Microsoft.EntityFrameworkCore;

namespace CoreWebApp.Pages
{
    public class CompaniesModel : PageModel
    {
        private readonly ILogger<CompaniesModel> _logger;
        private readonly ApplicationDbContext _context;
        private readonly IAuthService _authService;

        public CompaniesModel(ILogger<CompaniesModel> logger, ApplicationDbContext context, IAuthService authService)
        {
            _logger = logger;
            _context = context;
            _authService = authService;
        }

        public string Message { get; set; } = string.Empty;
        public bool Success { get; set; }
        public int CompaniesCount { get; set; }
        public List<Company> Companies { get; set; } = new List<Company>();

        public async Task<IActionResult> OnGetAsync()
        {
            try
            {
                // Check if user is authenticated
                var isAuthenticated = await _authService.IsUserAuthenticatedAsync();
                if (!isAuthenticated)
                {
                    Message = "Please log in to view companies.";
                    Success = false;
                    CompaniesCount = 0;
                    return Page();
                }

                // Get current user
                var currentUser = await _authService.GetCurrentUserAsync();
                if (currentUser == null)
                {
                    Message = "User session not found. Please log in again.";
                    Success = false;
                    CompaniesCount = 0;
                    return Page();
                }

                // Try to load companies directly - if it fails, the table doesn't exist
                try
                {
                    Companies = await _context.Companies
                        .Include(c => c.Chatbots)
                        .Include(c => c.LLMs)
                        .OrderBy(c => c.Name)
                        .ToListAsync();
                    
                    CompaniesCount = Companies.Count;
                    
                    // Log that user is viewing companies
                    _logger.LogInformation("User {Email} viewed companies page. Found {Count} companies.", 
                        currentUser.Email, CompaniesCount);
                    
                    Success = true;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error loading companies - table may not exist");
                    CompaniesCount = 0;
                    Message = "Database tables not found. Please import data first.";
                    Success = false;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading companies");
                Message = "Error loading companies from database.";
                Success = false;
                CompaniesCount = 0;
            }

            return Page();
        }
    }
} 