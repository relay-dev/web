using Microservices.Warmup;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;

namespace Microservices.AzureFunctions.Bootstrap
{
    public static class FunctionsHostBuilderExtensions
    {
        public static IFunctionsHostBuilder AddAzureFunctionsFramework(this IFunctionsHostBuilder builder, AzureFunctionsConfiguration config)
        {
            return new AzureFunctionsBootstrapper(config).Configure(builder);
        }

        public static IFunctionsHostBuilder AddWarmupType<TWarmup>(this IFunctionsHostBuilder builder)
        {
            builder.Services.AddSingleton(typeof(TWarmup));

            WarmupTasks.AddWarmupType(typeof(TWarmup));

            return builder;
        }
    }
}
