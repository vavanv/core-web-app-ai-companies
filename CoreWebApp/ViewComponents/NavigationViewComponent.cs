using Microsoft.AspNetCore.Mvc;
using CoreWebApp.Services;

namespace CoreWebApp.ViewComponents
{
    public class NavigationViewComponent : ViewComponent
    {
        private readonly IAuthHelperService _authHelperService;

        public NavigationViewComponent(IAuthHelperService authHelperService)
        {
            _authHelperService = authHelperService;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var isAuthenticated = await _authHelperService.IsUserAuthenticatedAsync();
            var currentUser = await _authHelperService.GetCurrentUserAsync();

            var model = new NavigationViewModel
            {
                IsAuthenticated = isAuthenticated,
                CurrentUser = currentUser
            };

            return View(model);
        }
    }

    public class NavigationViewModel
    {
        public bool IsAuthenticated { get; set; }
        public Models.User? CurrentUser { get; set; }
    }
} 