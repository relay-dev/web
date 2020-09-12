using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Web.AzureFunctions.Configuration;

namespace Web.AzureFunctions
{
    public static class ServiceCollectionExtensions
    {
        public static IFunctionsHostBuilder AddAzureFunctionsFramework(this IFunctionsHostBuilder builder, AzureFunctionsConfiguration config)
        {
            // Add Web Framework
            builder.Services.AddWebFramework(config);

            return builder;
        }
    }
}
