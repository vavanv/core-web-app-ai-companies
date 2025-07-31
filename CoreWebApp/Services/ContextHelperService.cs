using CoreWebApp.Models;
using Microsoft.AspNetCore.Http;

namespace CoreWebApp.Services
{
    public class ContextHelperService : IContextHelperService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ContextHelperService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public bool IsAuthenticated
        {
            get
            {
                var context = _httpContextAccessor.HttpContext;
                if (context?.Items.ContainsKey("IsAuthenticated") == true)
                {
                    var value = context.Items["IsAuthenticated"];
                    return value != null && (bool)value;
                }
                return false;
            }
        }

        public User? CurrentUser
        {
            get
            {
                var context = _httpContextAccessor.HttpContext;
                if (context?.Items.ContainsKey("CurrentUser") == true)
                {
                    return context.Items["CurrentUser"] as User;
                }
                return null;
            }
        }
    }
}