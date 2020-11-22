using Core.Plugins;
using Core.Plugins.AutoMapper;
using Core.Plugins.Azure;
using Core.Plugins.Configuration;
using Core.Plugins.EntityFramework;
using Core.Plugins.Framework;
using Core.Plugins.MediatR;
using Core.Utilities;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Net.Http;
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
                .AddApiExplorer()
                .AddNewtonsoftJson(options =>
                {
                    options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                    options.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;
                });

            // Add Core Plugins
            services.AddApplicationConfiguration(webConfiguration);
            services.AddCorePlugins(webConfiguration);
            services.AddAutoMapperPlugin(webConfiguration);
            services.AddAzureBlobStoragePlugin(webConfiguration);
            services.AddAzureEventGridPlugin(webConfiguration);
            services.AddEntityFrameworkPlugin();
            services.AddFluentValidationPlugin(mvcBuilder, webConfiguration);
            services.AddMediatRPlugin(webConfiguration);
            services.AddWarmup(webConfiguration);

            // Add WebConfiguration and ApplicationContext
            services.AddSingleton(webConfiguration);

            // Add Web services
            services.AddScoped<IJsonSerializer, NewtonsoftJsonSerializer>();

            // Add Diagnostics
            if (webConfiguration.IsAddDiagnostics)
            {
                mvcBuilder.PartManager.ApplicationParts.Add(new AssemblyPart(typeof(DiagnosticsController).Assembly));
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

            var pathBase = webConfiguration.Configuration["PATH_BASE"];

            if (!string.IsNullOrEmpty(pathBase))
            {
                app.UsePathBase(pathBase);
            }

            app.UseMiddleware<ErrorHandlingMiddleware>();
            app.UseMiddleware<UsernameReceiverMiddleware>();

            app.UseHttpsRedirection();
            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHealthChecks("/health");
            });

            if (webConfiguration.WarmupTypes.Any() && !webConfiguration.IsLocal())
            {
                var warmupExecutor = new WarmupTaskExecutor(app.ApplicationServices, webConfiguration);

                Task.Factory.StartNew(() => warmupExecutor.RunAsync(new CancellationToken()));
            }

            return app;
        }

        public static IServiceCollection AddWebFramework<TDbContext>(this IServiceCollection services, WebConfiguration webConfiguration) where TDbContext : DbContext
        {
            services = AddWebFramework(services, webConfiguration);

            services.AddDbContextUtilities<TDbContext>();

            return services;
        }

        public static IServiceCollection AddDbContextUtilities<TDbContext>(this IServiceCollection services) where TDbContext : DbContext
        {
            services.AddDbContext<TDbContext>();
            services.AddScoped<DbContext, TDbContext>();
            services.AddScoped<IDbContextProvider, DbContextProvider>();

            return services;
        }

        public static IServiceCollection AddScopedApiClient<TService, TImplementation>(this IServiceCollection services, string baseUrlSettingName) where TService : class where TImplementation : class, TService
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

        public static IServiceCollection AddFluentValidationPlugin(this IServiceCollection services, IMvcCoreBuilder mvcCoreBuilder, PluginConfiguration pluginConfiguration)
        {
            if (pluginConfiguration.ValidatorAssemblies == null || !pluginConfiguration.ValidatorAssemblies.Any())
            {
                return services;
            }

            mvcCoreBuilder.AddFluentValidation(fv => fv.RegisterValidatorsFromAssemblies(pluginConfiguration.ValidatorAssemblies));

            return services;
        }
    }
}
