using CoreWebApp.Models;

namespace CoreWebApp.Services
{
    public interface IContextHelperService
    {
        bool IsAuthenticated { get; }
        User? CurrentUser { get; }
    }
}