using Microservices.AzureFunctions.Configuration;
using Microservices.Warmup;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;

namespace Microservices.AzureFunctions
{
    public static class ServiceCollectionExtensions
    {
        public static IFunctionsHostBuilder AddAzureFunctionsFramework(this IFunctionsHostBuilder builder, AzureFunctionsConfiguration configuration)
        {
            // Configure services common to both Microservices and Azure Functions
            new MicroserviceBootstrapper().ConfigureCommonServices(builder.Services, configuration);

            // Add the Azure Functions configuration
            builder.Services.AddSingleton(configuration);

            return builder;
        }

        public static IFunctionsHostBuilder AddWarmupType<TWarmup>(this IFunctionsHostBuilder builder)
        {
            builder.Services.AddSingleton(typeof(TWarmup));

            WarmupTasks.AddWarmupType(typeof(TWarmup));

            return builder;
        }
    }
}
