using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Web.AzureFunctions.Configuration;

namespace Web.AzureFunctions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddAzureFunctionsFramework(this IServiceCollection services, AzureFunctionsConfiguration azureFunctionsConfiguration)
        {
            // Add Web Framework
            services.AddWebFramework(azureFunctionsConfiguration.WebConfiguration);

            // Add AzureFunctionsConfiguration
            services.AddSingleton(azureFunctionsConfiguration);

            return services;
        }

        public static IServiceCollection AddAzureFunctionsFramework<TDbContext>(this IServiceCollection services, AzureFunctionsConfiguration azureFunctionsConfiguration) where TDbContext : DbContext
        {
            services = AddAzureFunctionsFramework(services, azureFunctionsConfiguration);

            services.AddDbContext<TDbContext>();

            return services;
        }
    }
}
