using Microsoft.Extensions.Configuration;
using Web.AzureFunctions.Configuration;

namespace Web.AzureFunctions.Extensions
{
    public static class ConfigurationBuilderExtensions
    {
        public static AzureFunctionsConfigurationBuilder AsAzureFunctionsConfiguration(this IConfigurationBuilder configurationBuilder)
        {
            if (new AzureFunctionsConfiguration().IsLocal())
            {
                configurationBuilder.AddJsonFile("appsettings.Local.json", false, true);
                configurationBuilder.AddJsonFile("C:\\Azure\\appsettings.KeyVault.json", true, true);
                configurationBuilder.AddJsonFile("secrets.json", true, true);
            }

            configurationBuilder.AddEnvironmentVariables();

            IConfiguration configuration = configurationBuilder.Build();

            return new AzureFunctionsConfigurationBuilder()
                .UseConfiguration(configuration);
        }
    }
}
