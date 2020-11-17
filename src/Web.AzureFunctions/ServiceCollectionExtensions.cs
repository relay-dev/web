using Core.Plugins.Providers;
using Core.Providers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Web.AzureFunctions.Configuration;

namespace Web.AzureFunctions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddAzureFunctionsFramework(this IServiceCollection services, AzureFunctionsConfiguration azureFunctionsConfiguration)
        {
            // Add Web Framework
            services.AddWebFramework(azureFunctionsConfiguration.WebConfiguration);

            // Add Logging
            services.AddLogging(azureFunctionsConfiguration);

            // Add AzureFunctionsConfiguration
            services.AddSingleton(azureFunctionsConfiguration);

            // Add Event Handler framework
            if (azureFunctionsConfiguration.IsEventHandler)
            {
                services.AddUsernameProvider(azureFunctionsConfiguration.WebConfiguration.ApplicationContext.ApplicationName);
            }

            return services;
        }

        public static IServiceCollection AddAzureFunctionsFramework<TDbContext>(this IServiceCollection services, AzureFunctionsConfiguration azureFunctionsConfiguration) where TDbContext : DbContext
        {
            services = AddAzureFunctionsFramework(services, azureFunctionsConfiguration);

            services.AddDbContext<TDbContext>();

            return services;
        }

        public static IServiceCollection AddLogging(this IServiceCollection services, AzureFunctionsConfiguration azureFunctionsConfiguration)
        {
            services.AddLogging(logging =>
            {
                logging.AddConfiguration(azureFunctionsConfiguration.ApplicationConfiguration.GetSection("Logging"));

                if (azureFunctionsConfiguration.IsLocal)
                {
                    logging.AddConsole();
                    logging.AddDebug();
                }
                else
                {
                    logging.AddApplicationInsights();
                    logging.AddAzureWebAppDiagnostics();
                }
            });

            return services;
        }

        public static IServiceCollection AddUsernameProvider(this IServiceCollection services, string username)
        {
            var usernameProvider = new UsernameProvider();

            usernameProvider.Set(username);

            services.AddSingleton<IUsernameProvider>(usernameProvider);

            return services;
        }

        /// <summary>
        /// Note: This is an extension method on the ConfigurationBuilder, not the ServiceCollection. It's handy to have it where all the other application init code is
        /// </summary>
        public static ConfigurationBuilder AddAzureFunctionsConfiguration<TStartup>(this ConfigurationBuilder configurationBuilder) where TStartup : class
        {
            if (new AzureFunctionsConfiguration().IsLocal)
            {
                configurationBuilder.AddJsonFile("appsettings.Local.json", false, true);
                configurationBuilder.AddJsonFile("C:\\Azure\\appsettings.KeyVault.json", true, true);
                configurationBuilder.AddJsonFile("secrets.json", true, true);
            }

            configurationBuilder.AddEnvironmentVariables();

            return configurationBuilder;
        }
    }
}
