using CoreWebApp.Services;
using Microsoft.AspNetCore.Http;

namespace CoreWebApp.Middleware
{
    public class AuthenticationMiddleware
    {
        private readonly RequestDelegate _next;

        public AuthenticationMiddleware(RequestDelegate next)
        {
            _next = next;
        }

                public async Task InvokeAsync(HttpContext context, IAuthService authService)
        {
            // Check if user is authenticated via session
            var isAuthenticated = await authService.IsUserAuthenticatedAsync();

            if (isAuthenticated)
            {
                // Get current user and add to context
                var currentUser = await authService.GetCurrentUserAsync();
                if (currentUser != null)
                {
                    // Add user info to HttpContext.Items for easy access in Razor Pages
                    context.Items["CurrentUser"] = currentUser;
                    context.Items["IsAuthenticated"] = true;

                    // Log authentication success (optional for debugging)
                    var logger = context.RequestServices.GetService<ILogger<AuthenticationMiddleware>>();
                    logger?.LogDebug("User {Email} authenticated via middleware", currentUser.Email);
                }
            }
            else
            {
                context.Items["IsAuthenticated"] = false;
                context.Items["CurrentUser"] = null;
            }

            // Continue to the next middleware
            await _next(context);
        }
    }

    // Extension method for easy registration
    public static class AuthenticationMiddlewareExtensions
    {
        public static IApplicationBuilder UseAuthenticationMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<AuthenticationMiddleware>();
        }
    }
}