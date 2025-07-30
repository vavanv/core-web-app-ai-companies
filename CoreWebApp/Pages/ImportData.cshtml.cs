using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using CoreWebApp.Services;
using CoreWebApp.Data;
using CoreWebApp.Models;
using Microsoft.EntityFrameworkCore;

namespace CoreWebApp.Pages
{
    public class ImportDataModel : PageModel
    {
        private readonly ILogger<ImportDataModel> _logger;
        private readonly ICsvImportService _csvImportService;
        private readonly ApplicationDbContext _context;

        public ImportDataModel(ILogger<ImportDataModel> logger, ICsvImportService csvImportService, ApplicationDbContext context)
        {
            _logger = logger;
            _csvImportService = csvImportService;
            _context = context;
        }

        public string Message { get; set; } = string.Empty;
        public bool Success { get; set; }
        public ImportResult? ImportResult { get; set; }
        public int CompaniesCount { get; set; }
        public int ChatbotsCount { get; set; }
        public int LLMsCount { get; set; }
        public int UsersCount { get; set; }
        public List<Company> SampleCompanies { get; set; } = new List<Company>();

        public async Task<IActionResult> OnGetAsync()
        {
            await LoadDatabaseStats();
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(string action)
        {
            try
            {
                switch (action)
                {
                    case "import":
                        ImportResult = await _csvImportService.ImportCsvDataAsync();
                        Success = ImportResult.Success;
                        Message = ImportResult.Message;
                        break;

                    case "clear":
                        await ClearAllData();
                        Success = true;
                        Message = "All data has been cleared successfully.";
                        break;
                }

                await LoadDatabaseStats();
                return Page();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during import action: {Action}", action);
                Success = false;
                Message = $"Error: {ex.Message}";
                await LoadDatabaseStats();
                return Page();
            }
        }

        private async Task LoadDatabaseStats()
        {
            try
            {
                // Check if tables exist before querying
                var companiesExist = await _context.Database.CanConnectAsync() &&
                                   await _context.Database.ExecuteSqlRawAsync("SELECT name FROM sqlite_master WHERE type='table' AND name='Companies'") > 0;

                if (companiesExist)
                {
                    CompaniesCount = await _context.Companies.CountAsync();
                    ChatbotsCount = await _context.Chatbots.CountAsync();
                    LLMsCount = await _context.LLMs.CountAsync();

                    if (CompaniesCount > 0)
                    {
                        SampleCompanies = await _context.Companies
                            .Include(c => c.Chatbots)
                            .Include(c => c.LLMs)
                            .Take(5)
                            .ToListAsync();
                    }
                }
                else
                {
                    CompaniesCount = 0;
                    ChatbotsCount = 0;
                    LLMsCount = 0;
                }

                UsersCount = await _context.Users.CountAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading database stats");
                CompaniesCount = 0;
                ChatbotsCount = 0;
                LLMsCount = 0;
                UsersCount = 0;
            }
        }

        private async Task ClearAllData()
        {
            _logger.LogInformation("Clearing all data from database...");

            // Clear in order to respect foreign key constraints
            _context.LLMs.RemoveRange(await _context.LLMs.ToListAsync());
            _context.Chatbots.RemoveRange(await _context.Chatbots.ToListAsync());
            _context.Companies.RemoveRange(await _context.Companies.ToListAsync());

            await _context.SaveChangesAsync();

            _logger.LogInformation("All data cleared successfully");
        }
    }
}