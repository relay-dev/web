using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Web.AzureFunctions.Configuration;

namespace Web.AzureFunctions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddAzureFunctionsFramework(this IServiceCollection services, AzureFunctionsConfiguration config)
        {
            // Add Web Framework
            services.AddWebFramework(config.WebConfiguration);

            return services;
        }

        public static IServiceCollection AddAzureFunctionsFramework<TDbContext>(this IServiceCollection services, AzureFunctionsConfiguration config) where TDbContext : DbContext
        {
            services = AddAzureFunctionsFramework(services, config);

            services.AddDbContext<TDbContext>();

            return services;
        }
    }
}
