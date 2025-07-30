using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using CoreWebApp.Data;
using CoreWebApp.Models;
using CoreWebApp.Services;
using Microsoft.EntityFrameworkCore;

namespace CoreWebApp.Pages
{
    public class DebugModel : PageModel
    {
        private readonly ILogger<DebugModel> _logger;
        private readonly ApplicationDbContext _context;
        private readonly IAuthService _authService;

        public DebugModel(ILogger<DebugModel> logger, ApplicationDbContext context, IAuthService authService)
        {
            _logger = logger;
            _context = context;
            _authService = authService;
        }

        public string Message { get; set; } = string.Empty;
        public bool Success { get; set; }
        public bool IsAuthenticated { get; set; }
        public string CurrentUser { get; set; } = "Not logged in";
        public string SessionUserId { get; set; } = "None";
        public bool DatabaseConnected { get; set; }
        public bool CompaniesTableExists { get; set; }
        public int CompaniesCount { get; set; }
        public int UsersCount { get; set; }
        public List<string> DatabaseTables { get; set; } = new List<string>();

        public async Task<IActionResult> OnGetAsync()
        {
            try
            {
                // Check authentication
                IsAuthenticated = await _authService.IsUserAuthenticatedAsync();
                var currentUser = await _authService.GetCurrentUserAsync();

                if (currentUser != null)
                {
                    CurrentUser = $"{currentUser.FullName} ({currentUser.Email})";
                }

                // Check session
                var httpContext = HttpContext;
                if (httpContext != null)
                {
                    SessionUserId = httpContext.Session.GetString("UserId") ?? "None";
                }

                // Check database connection
                DatabaseConnected = await _context.Database.CanConnectAsync();

                // Check if Companies table exists
                CompaniesTableExists = await _context.Database.ExecuteSqlRawAsync(
                    "SELECT name FROM sqlite_master WHERE type='table' AND name='Companies'") > 0;

                // Get table counts
                if (DatabaseConnected)
                {
                    try
                    {
                        UsersCount = await _context.Users.CountAsync();

                        if (CompaniesTableExists)
                        {
                            CompaniesCount = await _context.Companies.CountAsync();
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error getting counts");
                    }
                }

                // Get all database tables
                if (DatabaseConnected)
                {
                    try
                    {
                        var tables = await _context.Database.ExecuteSqlRawAsync(
                            "SELECT name FROM sqlite_master WHERE type='table'");

                        // For SQLite, we need to get the actual table names
                        var tableNames = new List<string>();
                        using var connection = _context.Database.GetDbConnection();
                        await connection.OpenAsync();
                        using var command = connection.CreateCommand();
                        command.CommandText = "SELECT name FROM sqlite_master WHERE type='table'";
                        using var reader = await command.ExecuteReaderAsync();

                        while (await reader.ReadAsync())
                        {
                            tableNames.Add(reader.GetString(0));
                        }

                        DatabaseTables = tableNames;
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error getting database tables");
                        DatabaseTables = new List<string> { "Error getting tables" };
                    }
                }

                Success = true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in debug page");
                Message = $"Error: {ex.Message}";
                Success = false;
            }

            return Page();
        }
    }
}