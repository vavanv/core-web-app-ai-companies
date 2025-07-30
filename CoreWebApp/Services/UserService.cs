using CoreWebApp.Models;
using CoreWebApp.Data;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;

namespace CoreWebApp.Services
{
    public class UserService : IUserService
    {
        private readonly ILogger<UserService> _logger;
        private readonly ApplicationDbContext _context;

        public UserService(ILogger<UserService> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public async Task<User?> AuthenticateUserAsync(string email, string password)
        {
            try
            {
                var user = await GetUserByEmailAsync(email);
                if (user == null || !user.IsActive)
                {
                    _logger.LogWarning("Authentication failed for email: {Email}", email);
                    return null;
                }

                if (VerifyPassword(password, user.PasswordHash))
                {
                    user.LastLoginAt = DateTime.UtcNow;
                    _logger.LogInformation("User authenticated successfully: {Email}", email);
                    return user;
                }

                _logger.LogWarning("Invalid password for user: {Email}", email);
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during authentication for email: {Email}", email);
                return null;
            }
        }

        public async Task<User?> GetUserByEmailAsync(string email)
        {
            return await _context.Users
                .FirstOrDefaultAsync(u => u.Email.ToLower() == email.ToLower());
        }

        public async Task<bool> CreateUserAsync(User user)
        {
            try
            {
                if (await GetUserByEmailAsync(user.Email) != null)
                {
                    _logger.LogWarning("User with email {Email} already exists", user.Email);
                    return false;
                }

                user.CreatedAt = DateTime.UtcNow;
                user.IsActive = true;
                user.PasswordHash = HashPassword(user.PasswordHash); // Assuming PasswordHash contains plain password

                _context.Users.Add(user);
                await _context.SaveChangesAsync();

                _logger.LogInformation("User created successfully: {Email}", user.Email);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating user: {Email}", user.Email);
                return false;
            }
        }

        public async Task<bool> UpdateUserAsync(User user)
        {
            try
            {
                var existingUser = await _context.Users.FindAsync(user.Id);
                if (existingUser == null)
                {
                    _logger.LogWarning("User not found for update: {Id}", user.Id);
                    return false;
                }

                existingUser.FirstName = user.FirstName;
                existingUser.LastName = user.LastName;
                existingUser.Email = user.Email;
                existingUser.IsActive = user.IsActive;

                await _context.SaveChangesAsync();
                _logger.LogInformation("User updated successfully: {Email}", user.Email);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating user: {Id}", user.Id);
                return false;
            }
        }

        public async Task<bool> DeleteUserAsync(int userId)
        {
            try
            {
                var user = await _context.Users.FindAsync(userId);
                if (user == null)
                {
                    _logger.LogWarning("User not found for deletion: {Id}", userId);
                    return false;
                }

                _context.Users.Remove(user);
                await _context.SaveChangesAsync();
                _logger.LogInformation("User deleted successfully: {Email}", user.Email);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting user: {Id}", userId);
                return false;
            }
        }

        public async Task<IEnumerable<User>> GetAllUsersAsync()
        {
            try
            {
                // Get all users first (for debugging)
                var allUsers = await _context.Users.ToListAsync();
                _logger.LogInformation("Total users in database: {Count}", allUsers.Count);

                // Then filter for active users
                var activeUsers = allUsers.Where(u => u.IsActive).ToList();
                _logger.LogInformation("Active users: {Count}", activeUsers.Count);

                return activeUsers;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all users");
                return new List<User>();
            }
        }

        public async Task<User?> GetUserByIdAsync(int id)
        {
            try
            {
                return await _context.Users.FindAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting user by ID: {Id}", id);
                return null;
            }
        }

        private static string HashPassword(string password)
        {
            using var sha256 = SHA256.Create();
            var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(hashedBytes);
        }

        private static bool VerifyPassword(string password, string hash)
        {
            var hashedPassword = HashPassword(password);
            return hashedPassword.Equals(hash, StringComparison.Ordinal);
        }
    }
}