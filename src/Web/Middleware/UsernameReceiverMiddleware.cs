using Core.Providers;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
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
            string username = TryGetValueOrDefault(context.Request.Headers, "X-Username");

            if (!string.IsNullOrEmpty(username))
            {
                usernameProvider.Set(username);
            }

            await _next(context);
        }

        public static TValue TryGetValueOrDefault<TKey, TValue>(IDictionary<TKey, TValue> dictionary, TKey key)
        {
            bool isSuccessful = dictionary.TryGetValue(key, out var value);

            return isSuccessful ? value : default;
        }
    }
}
