using Core.Providers;
using Extensions;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace Web.Middleware
{
    public class UsernameReceiverMiddleware
    {
        private readonly IUsernameProvider _usernameProvider;
        private readonly RequestDelegate _next;

        public UsernameReceiverMiddleware(IUsernameProvider usernameProvider, RequestDelegate next)
        {
            _usernameProvider = usernameProvider;
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            string username = context.Request.Headers.TryGetValueOrDefault("X-Username");

            if (!string.IsNullOrEmpty(username))
            {
                _usernameProvider.Set(username);
            }

            await _next(context);
        }
    }
}
