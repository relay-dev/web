using Core.Plugins;
using Core.Plugins.AutoMapper;
using Core.Plugins.Azure;
using Core.Plugins.EntityFramework;
using Core.Plugins.Framework;
using Core.Plugins.MediatR;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Web.Configuration;
using Web.Middleware;

namespace Web
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddWebFramework(this IServiceCollection services, WebConfiguration config)
        {
            // Add MVC and Newtonsoft
            IMvcCoreBuilder mvcBuilder = services
                .AddMvcCore()
                .AddNewtonsoftJson(options => options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore);

            // Add Core Plugins
            services.AddDefaultCorePlugins();
            services.AddAutoMapperPlugin(config.MapperTypes);
            services.AddAzureBlobStoragePlugin(config.Configuration);
            services.AddAzureEventGridPlugin(config.Configuration);
            services.AddEntityFrameworkPlugin();
            services.AddFluentValidationPlugin(mvcBuilder, config.ValidatorsAssembly);
            services.AddMediatRPlugin(config.CommandHandlerTypes);
            services.AddWarmup(config.WarmupTypes);

            // Add WebConfiguration and ApplicationContext
            services.AddSingleton(config);
            services.AddSingleton(config.Configuration);
            services.AddSingleton(config.ApplicationContext);

            return services;
        }

        public static IApplicationBuilder UseWebFramework(this IApplicationBuilder app, WebConfiguration configuration, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            var pathBase = configuration.Configuration["PATH_BASE"];

            if (!string.IsNullOrEmpty(pathBase))
            {
                app.UsePathBase(pathBase);
            }

            app.UseMiddleware(typeof(ErrorHandlingMiddleware));

            app.UseHttpsRedirection();
            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHealthChecks("/health");
            });

            if (configuration.WarmupTypes.Any() && !IsLocal)
            {
                var warmupExecutor = app.ApplicationServices.GetRequiredService<WarmupTaskExecutor>();

                Task.Factory.StartNew(() => warmupExecutor.RunAsync(new CancellationToken()));
            }

            return app;
        }

        public static IServiceCollection AddFluentValidationPlugin(this IServiceCollection services, IMvcCoreBuilder mvcCoreBuilder, Assembly assemblyToScan)
        {
            if (assemblyToScan == null)
            {
                return services;
            }

            // Add FluentValidation
            mvcCoreBuilder.AddFluentValidation(fv => fv.RegisterValidatorsFromAssembly(assemblyToScan));

            return services;
        }

        private static bool IsLocal => bool.Parse(Environment.GetEnvironmentVariable("IS_LOCAL") ?? false.ToString());
    }
}
