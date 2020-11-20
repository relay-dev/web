using Microsoft.Extensions.Configuration;
using Web.AzureFunctions.Configuration;

namespace Web.AzureFunctions.Extensions
{
    public static class ConfigurationBuilderExtensions
    {
        public static AzureFunctionsConfigurationBuilder AsAzureFunctionsConfiguration(this ConfigurationBuilder configurationBuilder)
        {
            IConfiguration configuration = configurationBuilder.Build();

            return new AzureFunctionsConfigurationBuilder()
                .UseConfiguration(configuration);
        }
    }
}
