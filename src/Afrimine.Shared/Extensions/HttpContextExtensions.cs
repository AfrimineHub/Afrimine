using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace Afrimine.Shared.Extensions
{
    public static class HttpContextExtensions
    {
        public static UserContext GetContext(this HttpContext context)
        {
            return new UserContext
            {
                Id = context.User.GetLoggedInUserId()!,
                Email = context.User.GetLoggedInUserEmail()!,
                IpAddress = context.GetUserIp()
            };
        }

        public static string? GetLoggedInUserId(this ClaimsPrincipal user)
        {
            return user?
                .FindFirst(ClaimTypes.NameIdentifier)?
                .Value;
        }

        public static string? GetLoggedInUserEmail(this ClaimsPrincipal user)
        {
            return user?
                .FindFirst(ClaimTypes.Name)?
                .Value;
        }

        public static string? GetUserIp(this HttpContext context)
        {
            var ip = context.Request.Headers["X-Forwarded-For"]
                .FirstOrDefault()?
                .Split(',')
                .FirstOrDefault()?.Trim();

            ip ??= context.Connection.RemoteIpAddress?.ToString();
            return ip;
        }
    }
}
