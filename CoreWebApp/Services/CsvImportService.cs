using CoreWebApp.Data;
using CoreWebApp.Models;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

namespace CoreWebApp.Services
{
    public class CsvImportService : ICsvImportService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<CsvImportService> _logger;
        private readonly IWebHostEnvironment _environment;

        public CsvImportService(ApplicationDbContext context, ILogger<CsvImportService> logger, IWebHostEnvironment environment)
        {
            _context = context;
            _logger = logger;
            _environment = environment;
        }

        public async Task<ImportResult> ImportCsvDataAsync()
        {
            var result = new ImportResult();

            try
            {
                _logger.LogInformation("Starting CSV import...");

                // Ensure database tables exist
                await _context.Database.EnsureCreatedAsync();
                _logger.LogInformation("Database tables ensured");

                // Check if data already exists
                if (await IsDataImportedAsync())
                {
                    result.Success = false;
                    result.Message = "Data has already been imported. Clear existing data first if you want to re-import.";
                    return result;
                }

                var csvPath = Path.Combine(_environment.ContentRootPath, "files", "spec_import.csv");

                if (!File.Exists(csvPath))
                {
                    result.Success = false;
                    result.Message = $"CSV file not found at: {csvPath}";
                    return result;
                }

                var companies = new Dictionary<string, Company>();
                var chatbots = new List<Chatbot>();
                var llms = new List<LLM>();

                // Read CSV file with proper file sharing
                string[] lines;
                try
                {
                    using var stream = new FileStream(csvPath, FileMode.Open, FileAccess.Read, FileShare.Read);
                    using var reader = new StreamReader(stream);
                    var content = await reader.ReadToEndAsync();
                    lines = content.Split('\n', StringSplitOptions.RemoveEmptyEntries);
                }
                catch (IOException ex)
                {
                    _logger.LogError(ex, "Cannot access CSV file. It may be open in another application.");
                    result.Success = false;
                    result.Message = "Cannot access the CSV file. Please close any applications that might have it open and try again.";
                    return result;
                }

                // Skip header row
                for (int i = 1; i < lines.Length; i++)
                {
                    var line = lines[i];
                    var columns = ParseCsvLine(line);

                    if (columns.Length >= 5)
                    {
                        var companyName = columns[0].Trim();
                        var description = columns[1].Trim();
                        var chatbotName = columns[2].Trim();
                        var llmName = columns[3].Trim();
                        var specialization = columns[4].Trim();

                        // Create or get company
                        if (!companies.ContainsKey(companyName))
                        {
                            var newCompany = new Company
                            {
                                Name = companyName,
                                Description = description,
                                CreatedAt = DateTime.UtcNow
                            };
                            companies[companyName] = newCompany;
                        }

                        var company = companies[companyName];

                        // Add chatbot if not "None"
                        if (!string.IsNullOrEmpty(chatbotName) && chatbotName.ToLower() != "none")
                        {
                            chatbots.Add(new Chatbot
                            {
                                Name = chatbotName,
                                CompanyId = company.Id,
                                CreatedAt = DateTime.UtcNow
                            });
                        }

                        // Add LLM if not empty
                        if (!string.IsNullOrEmpty(llmName))
                        {
                            llms.Add(new LLM
                            {
                                Name = llmName,
                                Specialization = specialization,
                                CompanyId = company.Id,
                                CreatedAt = DateTime.UtcNow
                            });
                        }
                    }
                }

                // Save to database
                await _context.Companies.AddRangeAsync(companies.Values);
                await _context.SaveChangesAsync();

                // Update CompanyId references for chatbots and LLMs
                // We need to get the actual company names that these belong to
                // For now, we'll use a simple approach - each chatbot/LLM belongs to the company in the same row
                // This is a simplified approach - in a real scenario, you might want more complex logic

                // Since we're processing row by row, we can track which company each item belongs to
                var chatbotCompanyMap = new Dictionary<string, string>(); // chatbot name -> company name
                var llmCompanyMap = new Dictionary<string, string>(); // llm name -> company name

                // Re-read the CSV to build the mappings
                try
                {
                    using var stream = new FileStream(csvPath, FileMode.Open, FileAccess.Read, FileShare.Read);
                    using var reader = new StreamReader(stream);
                    var csvContent = await reader.ReadToEndAsync();
                    var csvLines = csvContent.Split('\n', StringSplitOptions.RemoveEmptyEntries);

                    for (int i = 1; i < csvLines.Length; i++)
                    {
                        var line = csvLines[i];
                        var columns = ParseCsvLine(line);

                        if (columns.Length >= 5)
                        {
                            var companyName = columns[0].Trim();
                            var chatbotName = columns[2].Trim();
                            var llmName = columns[3].Trim();

                            if (!string.IsNullOrEmpty(chatbotName) && chatbotName.ToLower() != "none")
                            {
                                chatbotCompanyMap[chatbotName] = companyName;
                            }

                            if (!string.IsNullOrEmpty(llmName))
                            {
                                llmCompanyMap[llmName] = companyName;
                            }
                        }
                    }
                }
                catch (IOException ex)
                {
                    _logger.LogError(ex, "Error re-reading CSV for mapping");
                    result.Success = false;
                    result.Message = "Error processing CSV file relationships.";
                    return result;
                }

                // Update chatbot company IDs
                foreach (var chatbot in chatbots)
                {
                    if (chatbotCompanyMap.TryGetValue(chatbot.Name, out var companyName))
                    {
                        var company = companies.Values.FirstOrDefault(c => c.Name == companyName);
                        if (company != null)
                        {
                            chatbot.CompanyId = company.Id;
                        }
                    }
                }

                // Update LLM company IDs
                foreach (var llm in llms)
                {
                    if (llmCompanyMap.TryGetValue(llm.Name, out var companyName))
                    {
                        var company = companies.Values.FirstOrDefault(c => c.Name == companyName);
                        if (company != null)
                        {
                            llm.CompanyId = company.Id;
                        }
                    }
                }

                await _context.Chatbots.AddRangeAsync(chatbots);
                await _context.LLMs.AddRangeAsync(llms);
                await _context.SaveChangesAsync();

                result.Success = true;
                result.CompaniesImported = companies.Count;
                result.ChatbotsImported = chatbots.Count;
                result.LLMsImported = llms.Count;
                result.Message = $"Successfully imported {companies.Count} companies, {chatbots.Count} chatbots, and {llms.Count} LLMs.";

                _logger.LogInformation("CSV import completed successfully: {Companies} companies, {Chatbots} chatbots, {LLMs} LLMs",
                    companies.Count, chatbots.Count, llms.Count);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during CSV import");
                result.Success = false;
                result.Message = $"Error importing CSV: {ex.Message}";
            }

            return result;
        }

        public async Task<bool> IsDataImportedAsync()
        {
            try
            {
                // Check if the Companies table exists first
                var tableExists = await _context.Database.ExecuteSqlRawAsync("SELECT name FROM sqlite_master WHERE type='table' AND name='Companies'") > 0;
                if (!tableExists)
                {
                    return false;
                }

                return await _context.Companies.AnyAsync();
            }
            catch
            {
                return false;
            }
        }

        private string[] ParseCsvLine(string line)
        {
            var result = new List<string>();
            var current = "";
            var inQuotes = false;

            for (int i = 0; i < line.Length; i++)
            {
                var c = line[i];

                if (c == '"')
                {
                    inQuotes = !inQuotes;
                }
                else if (c == ',' && !inQuotes)
                {
                    result.Add(current);
                    current = "";
                }
                else
                {
                    current += c;
                }
            }

            result.Add(current);
            return result.ToArray();
        }
    }
}