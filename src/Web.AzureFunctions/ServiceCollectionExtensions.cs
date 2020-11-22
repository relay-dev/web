using Core.Plugins;
using Core.Plugins.Providers;
using Core.Providers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Linq;
using Web.AzureFunctions.Configuration;

namespace Web.AzureFunctions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddAzureFunctionsFramework(this IServiceCollection services, AzureFunctionsConfiguration azureFunctionsConfiguration)
        {
            // Add Web Framework
            services.AddWebFramework(azureFunctionsConfiguration);

            // Add Logging
            services.AddLogging(azureFunctionsConfiguration);

            // Add AzureFunctionsConfiguration
            services.AddSingleton(azureFunctionsConfiguration);

            // Add Functions
            if (azureFunctionsConfiguration.FunctionTypes.Any())
            {
                services.AddTypes(azureFunctionsConfiguration.FunctionTypes);
            }

            // Add Event Handler framework
            if (azureFunctionsConfiguration.IsEventHandler)
            {
                services.AddUsernameProvider(azureFunctionsConfiguration.ApplicationName);
            }

            return services;
        }

        public static IServiceCollection AddAzureFunctionsFramework<TDbContext>(this IServiceCollection services, AzureFunctionsConfiguration azureFunctionsConfiguration) where TDbContext : DbContext
        {
            services = AddAzureFunctionsFramework(services, azureFunctionsConfiguration);

            services.AddDbContextUtilities<TDbContext>();

            return services;
        }

        public static IServiceCollection AddLogging(this IServiceCollection services, AzureFunctionsConfiguration azureFunctionsConfiguration)
        {
            services.AddLogging(logging =>
            {
                logging.AddConfiguration(azureFunctionsConfiguration.Configuration.GetSection("Logging"));

                if (azureFunctionsConfiguration.IsLocal())
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
    }
}
