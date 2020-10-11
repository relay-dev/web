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
    }
}
