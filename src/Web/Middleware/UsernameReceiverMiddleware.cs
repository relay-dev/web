using Core.Providers;
using Extensions;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace Web.Middleware
{
    public class UsernameReceiverMiddleware
    {
        private readonly RequestDelegate _next;

        public UsernameReceiverMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context, IUsernameProvider usernameProvider)
        {
            string username = context.Request.Headers.TryGetValueOrDefault("X-Username");

            if (!string.IsNullOrEmpty(username))
            {
                usernameProvider.Set(username);
            }

            await _next(context);
        }
    }
}
