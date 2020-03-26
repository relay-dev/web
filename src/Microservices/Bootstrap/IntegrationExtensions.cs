using Microservices.Warmup;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace Microservices.Bootstrap
{
    public static class IntegrationExtensions
    {
        public static IServiceCollection AddMicroserviceFramework(this IServiceCollection services, MicroserviceConfiguration config)
        {
            new MicroserviceBootstrapper(config).ConfigureServices(services);

            return services;
        }

        public static IApplicationBuilder UseMicroserviceFramework(this IApplicationBuilder app, MicroserviceConfiguration config, IWebHostEnvironment env)
        {
            new MicroserviceBootstrapper(config).Configure(app, env);

            return app;
        }

        public static IServiceCollection AddWarmupType<TWarmup>(this IServiceCollection services)
        {
            services.AddSingleton(typeof(TWarmup));

            WarmupTasks.AddWarmupType(typeof(TWarmup));

            return services;
        }
    }
}
