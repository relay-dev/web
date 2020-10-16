using Core.Framework;
using Core.Providers;
using Extensions;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace Web.Middleware
{
    public class UsernameReceiverMiddleware
    {
        private readonly ICommandContextProvider _commandContextProvider;
        private readonly RequestDelegate _next;

        public UsernameReceiverMiddleware(ICommandContextProvider commandContextProvider, RequestDelegate next)
        {
            _commandContextProvider = commandContextProvider;
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            string username = context.Request.Headers.TryGetValueOrDefault("x-username");

            if (!string.IsNullOrEmpty(username))
            {
                var commandContext = new CommandContext
                {
                    Username = username
                };

                _commandContextProvider.Set(commandContext);
            }

            await _next(context);
        }
    }
}
