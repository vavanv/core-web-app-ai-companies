using CoreWebApp.Models;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;

namespace CoreWebApp.Data
{
    public class DbInitializer : IDbInitializer
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<DbInitializer> _logger;

        public DbInitializer(ApplicationDbContext context, ILogger<DbInitializer> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task InitializeAsync()
        {
            try
            {
                // Ensure database is created
                await _context.Database.EnsureCreatedAsync();

                // Check if we already have users
                var existingUsers = await _context.Users.ToListAsync();
                _logger.LogInformation("Found {Count} existing users in database", existingUsers.Count);

                if (!existingUsers.Any())
                {
                    _logger.LogInformation("Seeding database with initial users...");

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

                    _logger.LogInformation("Database seeded successfully with {Count} users", users.Count);
                }
                else
                {
                    _logger.LogInformation("Database already contains users, skipping seed");
                    foreach (var user in existingUsers)
                    {
                        _logger.LogInformation("Existing user: {Email} - Active: {IsActive}", user.Email, user.IsActive);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error initializing database");
                throw;
            }
        }

        private static string HashPassword(string password)
        {
            using var sha256 = SHA256.Create();
            var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(hashedBytes);
        }
    }
}