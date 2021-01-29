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
            services.AddApplicationServices(webConfiguration);
            services.AddCorePlugins(webConfiguration);
            services.AddAutoMapperPlugin(webConfiguration);
            services.AddAzureBlobStoragePlugin(webConfiguration);
            services.AddAzureEventGridPlugin(webConfiguration);
            services.AddEntityFrameworkPlugin(webConfiguration);
            services.AddFluentValidationPlugin(webConfiguration, mvcBuilder);
            services.AddMediatRPlugin(webConfiguration);
            services.AddWarmup(webConfiguration);

            // Add WebConfiguration and ApplicationContext
            services.AddSingleton(webConfiguration);

            // Add Web services
            services.Add<IJsonSerializer, NewtonsoftJsonSerializer>(webConfiguration.ServiceLifetime);

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

            services.AddDbContextUtilities<TDbContext>(webConfiguration);

            return services;
        }

        public static IServiceCollection AddDbContextUtilities<TDbContext>(this IServiceCollection services, WebConfiguration webConfiguration) where TDbContext : DbContext
        {
            if (webConfiguration.IsEnableRetryOnDbContextFailure)
            {
                services.AddDbContext<TDbContext>(options => options.UseSqlServer(opt => opt.EnableRetryOnFailure()));
            }
            else
            {
                services.AddDbContext<TDbContext>();
            }

            services.Add<DbContext, TDbContext>(webConfiguration.ServiceLifetime);

            return services;
        }

        public static IServiceCollection AddApiClient<TService, TImplementation>(this IServiceCollection services, string baseUrlSettingName, ServiceLifetime serviceLifetime = ServiceLifetime.Scoped) where TService : class where TImplementation : class, TService
        {
            services.Add<TService>(sp =>
            {
                string apiBaseUrl = sp.GetRequiredService<IConfiguration>()[baseUrlSettingName];

                var httpClient = new HttpClient
                {
                    BaseAddress = new Uri(apiBaseUrl)
                };

                return (TImplementation)Activator.CreateInstance(typeof(TImplementation), httpClient);
            }, serviceLifetime);

            return services;
        }

        public static IServiceCollection AddFluentValidationPlugin(this IServiceCollection services, PluginConfiguration pluginConfiguration, IMvcCoreBuilder mvcCoreBuilder)
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
