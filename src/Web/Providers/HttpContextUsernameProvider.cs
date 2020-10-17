using Core.Providers;
using Microsoft.AspNetCore.Http;

namespace Web.Providers
{
    public class HttpContextUsernameProvider : IUsernameProvider
    {
        private readonly IHttpContextAccessor _httpContext;

        public HttpContextUsernameProvider(IHttpContextAccessor httpContext)
        {
            _httpContext = httpContext;
        }

        public string Get()
        {
            return _httpContext.HttpContext.Items["Username"].ToString();
        }

        public void Set(string username)
        {
            _httpContext.HttpContext.Items["Username"] = username;
        }
    }
}
