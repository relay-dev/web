using Core.Plugins;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Linq;
using Web.AzureFunctions.Configuration;
using Web.AzureFunctions.Framework;

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
                services.AddSingletonUsernameProvider(azureFunctionsConfiguration.ApplicationName);
            }

            // Add types needed for Azure Functions
            services.Add<IAzureFunctionsCommandExecutor, AzureFunctionsCommandExecutor>(azureFunctionsConfiguration.ServiceLifetime);

            return services;
        }

        public static IServiceCollection AddAzureFunctionsFramework<TDbContext>(this IServiceCollection services, AzureFunctionsConfiguration azureFunctionsConfiguration) where TDbContext : DbContext
        {
            services = AddAzureFunctionsFramework(services, azureFunctionsConfiguration);

            services.AddDbContextUtilities<TDbContext>(azureFunctionsConfiguration);

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
    }
}
