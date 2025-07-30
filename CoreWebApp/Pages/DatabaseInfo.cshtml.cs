using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using CoreWebApp.Services;
using CoreWebApp.Models;
using CoreWebApp.Data;
using System.IO;

namespace CoreWebApp.Pages
{
    public class DatabaseInfoModel : PageModel
    {
        private readonly ILogger<DatabaseInfoModel> _logger;
        private readonly IUserService _userService;
        private readonly ApplicationDbContext _context;

        public DatabaseInfoModel(ILogger<DatabaseInfoModel> logger, IUserService userService, ApplicationDbContext context)
        {
            _logger = logger;
            _userService = userService;
            _context = context;
        }

        public string DatabasePath { get; set; } = string.Empty;
        public bool DatabaseExists { get; set; }
        public int TotalUsers { get; set; }
        public int ActiveUsers { get; set; }
        public IEnumerable<User>? Users { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            try
            {
                // Get database file path
                DatabasePath = Path.Combine(Directory.GetCurrentDirectory(), "CoreWebApp.db");
                DatabaseExists = System.IO.File.Exists(DatabasePath);

                // Get all users
                Users = await _context.Users.ToListAsync();
                TotalUsers = Users.Count();
                ActiveUsers = Users.Count(u => u.IsActive);

                _logger.LogInformation("Database info - Path: {Path}, Exists: {Exists}, TotalUsers: {Total}, ActiveUsers: {Active}",
                    DatabasePath, DatabaseExists, TotalUsers, ActiveUsers);

                return Page();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting database information");
                return RedirectToPage("/Error");
            }
        }

        public async Task<IActionResult> OnPostAsync(string action)
        {
            try
            {
                switch (action)
                {
                    case "seed":
                        await ReseedDatabase();
                        break;
                    case "clear":
                        await ClearDatabase();
                        break;
                }

                return RedirectToPage();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error performing database action: {Action}", action);
                return RedirectToPage("/Error");
            }
        }

        private async Task ReseedDatabase()
        {
            _logger.LogInformation("Re-seeding database...");

            // Clear existing users
            var existingUsers = await _context.Users.ToListAsync();
            _context.Users.RemoveRange(existingUsers);
            await _context.SaveChangesAsync();

            // Add demo users
            var users = new List<User>
            {
                new User
                {
                    Email = "admin@example.com",
                    PasswordHash = HashPassword("password123"),
                    FirstName = "Admin",
                    LastName = "User",
                    CreatedAt = DateTime.UtcNow.AddDays(-30),
                    IsActive = true
                },
                new User
                {
                    Email = "user@example.com",
                    PasswordHash = HashPassword("password123"),
                    FirstName = "John",
                    LastName = "Doe",
                    CreatedAt = DateTime.UtcNow.AddDays(-15),
                    IsActive = true
                }
            };

            await _context.Users.AddRangeAsync(users);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Database re-seeded successfully with {Count} users", users.Count);
        }

        private async Task ClearDatabase()
        {
            _logger.LogInformation("Clearing all users from database...");

            var existingUsers = await _context.Users.ToListAsync();
            _context.Users.RemoveRange(existingUsers);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Database cleared successfully");
        }

        private static string HashPassword(string password)
        {
            using var sha256 = System.Security.Cryptography.SHA256.Create();
            var hashedBytes = sha256.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(hashedBytes);
        }
    }
}