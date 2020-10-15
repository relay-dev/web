using Web.Configuration;
using Web.Rest.Configuration;

namespace Web.Rest.Extensions
{
    public static class WebConfigurationBuilderExtensions
    {
        public static RestConfigurationBuilder AsRestConfiguration(this WebConfigurationBuilder webConfigurationBuilder)
        {
            return new RestConfigurationBuilder(webConfigurationBuilder);
        }
    }
}
