using Core.Plugins.Configuration;
using Microsoft.Extensions.Configuration;
using System;
using Web.AzureFunctions.Configuration;

namespace Web.AzureFunctions.Extensions
{
    public static class ConfigurationBuilderExtensions
    {
        public static AzureFunctionsConfigurationBuilder AsAzureFunctionsConfiguration(this IConfigurationBuilder configBuilder)
        {
            string environmentName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

            // Load the appsettings file and overlay environment specific configurations
            configBuilder
                .AddJsonFile("appsettings.json", true, true)
                .AddJsonFile($"appsettings.{environmentName}.json", true, true);

            // Load application secrets
            if (IsLocal)
            {
                configBuilder
                    .AddJsonFile("appsettings.Local.json", false, true)
                    .AddJsonFile("local.settings.json", true, true)
                    .AddJsonFile("C:\\Azure\\appsettings.KeyVault.json", true, true)
                    .AddJsonFile("secrets.json", true, true);
            }

            configBuilder.AddEnvironmentVariables();

            return new AzureFunctionsConfigurationBuilder(configBuilder.Build());
        }

        public static bool IsLocal => new ApplicationConfiguration().IsLocal();
    }
}
