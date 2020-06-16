using Microservices.Bootstrap;
using Microservices.Warmup;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace Microservices
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddMicroserviceFramework(this IServiceCollection services, MicroserviceConfiguration config)
        {
            return new MicroserviceBootstrapper(config).ConfigureServices(services);
        }

        public static IApplicationBuilder UseMicroserviceFramework(this IApplicationBuilder app, MicroserviceConfiguration config, IWebHostEnvironment env)
        {
            return new MicroserviceBootstrapper(config).Configure(app, env);
        }

        public static IServiceCollection AddWarmupType<TWarmup>(this IServiceCollection services)
        {
            services.AddSingleton(typeof(TWarmup));

            WarmupTasks.AddWarmupType(typeof(TWarmup));

            return services;
        }
    }
}
