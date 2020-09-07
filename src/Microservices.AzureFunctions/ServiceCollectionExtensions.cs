using Microservices.AzureFunctions.Configuration;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;

namespace Microservices.AzureFunctions
{
    public static class ServiceCollectionExtensions
    {
        public static IFunctionsHostBuilder AddAzureFunctionsFramework(this IFunctionsHostBuilder builder, AzureFunctionsConfiguration configuration)
        {
            // Configure services common to both Microservices and Azure Functions
            builder.Services.AddWebFramework(configuration);

            // Add the Azure Functions configuration
            builder.Services.AddSingleton(configuration);

            return builder;
        }
    }
}
