namespace CoreWebApp.Data
{
    public interface IDbInitializer
    {
        Task InitializeAsync();
    }
}