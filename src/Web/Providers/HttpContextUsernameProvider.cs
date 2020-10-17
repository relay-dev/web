using Core.Providers;
using Microsoft.AspNetCore.Http;

namespace Web.Providers
{
    public class HttpContextUsernameProvider : IUsernameProvider
    {
        private readonly HttpContext _httpContext;

        public HttpContextUsernameProvider(IHttpContextAccessor httpContext)
        {
            _httpContext = httpContext.HttpContext;
        }

        public string Get()
        {
            return _httpContext.Items["Username"].ToString();
        }

        public void Set(string username)
        {
            _httpContext.Items["Username"] = username;
        }
    }
}
