namespace CoreWebApp.Services
{
    public interface ICsvImportService
    {
        Task<ImportResult> ImportCsvDataAsync();
        Task<bool> IsDataImportedAsync();
    }

    public class ImportResult
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public int CompaniesImported { get; set; }
        public int ChatbotsImported { get; set; }
        public int LLMsImported { get; set; }
    }
}