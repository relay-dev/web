using Microsoft.Extensions.Configuration;
using Web.AzureFunctions.Configuration;

namespace Web.AzureFunctions.Extensions
{
    public static class ConfigurationExtensions
    {
        public static AzureFunctionsConfigurationBuilder AsAzureFunctionsConfiguration(this IConfiguration configuration)
        {
            return new AzureFunctionsConfigurationBuilder(configuration);
        }
    }
}
