using Core.Plugins;
using Core.Plugins.AutoMapper;
using Core.Plugins.Azure;
using Core.Plugins.EntityFramework;
using Core.Plugins.Framework;
using Core.Plugins.MediatR;
using Core.Providers;
using Core.Utilities;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Web.Configuration;
using Web.Controllers;
using Web.Middleware;
using Web.Providers;
using Web.Serialization;

namespace Web
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddWebFramework(this IServiceCollection services, WebConfiguration webConfiguration)
        {
            // Add MVC and Newtonsoft
            IMvcCoreBuilder mvcBuilder = services
                .AddMvcCore()
                .AddApplicationPart(typeof(DiagnosticsController<>).Assembly)
                .AddNewtonsoftJson(options => options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore);

            // Add Core Plugins
            services.AddDefaultCorePlugins();
            services.AddAutoMapperPlugin(webConfiguration.MapperTypes);
            services.AddAzureBlobStoragePlugin(webConfiguration.ApplicationConfiguration);
            services.AddAzureEventGridPlugin(webConfiguration.ApplicationConfiguration);
            services.AddEntityFrameworkPlugin();
            services.AddFluentValidationPlugin(mvcBuilder, webConfiguration.ValidatorsAssembly);
            services.AddMediatRPlugin(webConfiguration.CommandHandlerTypes);
            services.AddWarmup(webConfiguration.WarmupTypes);

            // Add WebConfiguration and ApplicationContext
            services.AddSingleton(webConfiguration);
            services.AddSingleton(webConfiguration.ApplicationConfiguration);
            services.AddSingleton(webConfiguration.ApplicationContext);

            // Add DistributedCache (NewtonsoftJsonSerializer is needed for the cache utility)
            services.AddDistributedMemoryCache();

            // Add framework
            services.AddTransient<UsernameReceiverMiddleware>();

            // Add overwrites
            services.AddScoped<IUsernameProvider, HttpContextUsernameProvider>();
            services.AddScoped<IJsonSerializer, NewtonsoftJsonSerializer>();

            // Add ApiExplorer
            if (webConfiguration.IsAddApiExplorer)
            {
                mvcBuilder.AddApiExplorer();
            }

            return services;
        }

        public static IApplicationBuilder UseWebFramework(this IApplicationBuilder app, WebConfiguration webConfiguration, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            var pathBase = webConfiguration.ApplicationConfiguration["PATH_BASE"];

            if (!string.IsNullOrEmpty(pathBase))
            {
                app.UsePathBase(pathBase);
            }

            app.UseMiddleware<ErrorHandlingMiddleware>();
            //app.UseMiddleware<UsernameReceiverMiddleware>();

            app.UseHttpsRedirection();
            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHealthChecks("/health");
            });

            if (webConfiguration.WarmupTypes.Any() && !IsLocal)
            {
                var warmupExecutor = new WarmupTaskExecutor(app.ApplicationServices, webConfiguration.WarmupTypes);

                Task.Factory.StartNew(() => warmupExecutor.RunAsync(new CancellationToken()));
            }

            return app;
        }

        public static IServiceCollection AddWebFramework<TDbContext>(this IServiceCollection services, WebConfiguration webConfiguration) where TDbContext : DbContext
        {
            services = AddWebFramework(services, webConfiguration);

            services.AddDbContext<TDbContext>();
            services.AddScoped<DiagnosticsController<TDbContext>>();

            return services;
        }

        public static IServiceCollection AddScopedService<TService, TImplementation>(this IServiceCollection services, string baseUrlSettingName) where TService : class where TImplementation : class, TService
        {
            services.AddScoped(typeof(TService), sp =>
            {
                string apiBaseUrl = sp.GetRequiredService<IConfiguration>()[baseUrlSettingName];

                var httpClient = new HttpClient
                {
                    BaseAddress = new Uri(apiBaseUrl)
                };

                return (TImplementation)Activator.CreateInstance(typeof(TImplementation), httpClient);
            });

            return services;
        }

        public static IServiceCollection AddFluentValidationPlugin(this IServiceCollection services, IMvcCoreBuilder mvcCoreBuilder, Assembly assemblyToScan)
        {
            if (assemblyToScan == null)
            {
                return services;
            }

            mvcCoreBuilder.AddFluentValidation(fv => fv.RegisterValidatorsFromAssembly(assemblyToScan));

            return services;
        }

        private static bool IsLocal => bool.Parse(Environment.GetEnvironmentVariable("IS_LOCAL") ?? false.ToString());
    }
}
