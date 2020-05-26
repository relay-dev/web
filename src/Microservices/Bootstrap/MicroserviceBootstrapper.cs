using AutoMapper;
using Core.Caching;
//using Core.Plugins.AutoMapper.Data.Resolvers.DatabaseResolver;
using Core.Plugins.Extensions;
using Core.Plugins.Microsoft.Azure.Storage;
using Core.Plugins.Microsoft.Azure.Storage.Impl;
using Core.Plugins.Microsoft.Azure.Wrappers;
using Core.Plugins.Providers;
using Core.Providers;
using FluentCommander.SqlServer;
using FluentValidation.AspNetCore;
using HealthChecks.UI.Client;
using MediatR;
using Microservices.Caching;
using Microservices.Middleware;
using Microservices.Serialization;
using Microservices.Serialization.Impl;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;

namespace Microservices.Bootstrap
{
    public class MicroserviceBootstrapper
    {
        protected readonly MicroserviceConfiguration _microserviceConfiguration;

        public MicroserviceBootstrapper(MicroserviceConfiguration microserviceConfiguration)
        {
            _microserviceConfiguration = microserviceConfiguration;
        }

        public IServiceCollection ConfigureServices(IServiceCollection services)
        {
            services
                .AddControllers()
                .AddFluentValidation();

            services
                .AddAutoMapper(cfg =>
                {
                    services.AddSingleton(provider =>
                    {
                        cfg.ConstructServicesUsing(type => ActivatorUtilities.CreateInstance(provider, type));
                        return cfg;
                    });
                }, _microserviceConfiguration.AssembliesToScan)
                .AddMediatR(_microserviceConfiguration.AssembliesToScan)
                .AddDistributedMemoryCache();

            services
                .AddHealthChecks();

            //services
            //    .AddApiVersioning(cfg =>
            //    {
            //        cfg.DefaultApiVersion = new ApiVersion(_microserviceConfiguration.SwaggerConfiguration.MajorVersion, _microserviceConfiguration.SwaggerConfiguration.MinorVersion);
            //        cfg.AssumeDefaultVersionWhenUnspecified = true;
            //        cfg.ReportApiVersions = true;
            //        cfg.ApiVersionReader = new HeaderApiVersionReader("x-api-version");
            //    });

            services
                .AddSwaggerGen(options =>
                {
                    options.SwaggerDoc(_microserviceConfiguration.ServiceName,
                        new OpenApiInfo
                        {
                            Title = _microserviceConfiguration.SwaggerConfiguration.Title,
                            Version = _microserviceConfiguration.SwaggerConfiguration.Version,
                            Description = _microserviceConfiguration.SwaggerConfiguration.Description
                        });
                });

            services
                .AddDatabaseCommander(_microserviceConfiguration.Configuration);

            //services.AddScoped(typeof(LookupDataKeyResolver<>));
            //services.AddScoped(typeof(LookupDataValueResolver<>));
            services.AddScoped<ICacheHelper, DistributedCacheHelper>();
            services.AddScoped<IStorageAccountFactory, AzureStorageAccountFactory>();
            services.AddScoped<IConnectionStringProvider, AzureConnectionStringByConfigurationProvider>();
            services.AddTransient<IntegrationTestConnectionStringProvider>();
            services.AddTransient<IJsonSerializer, NewtonsoftJsonSerializer>();
            services.AddSingleton<Warmup.Warmup>();
            services.AddSingleton<IApplicationContextProvider>(sp => new ApplicationContextProvider(_microserviceConfiguration.ApplicationContext));

            return services;
        }

        public IApplicationBuilder Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            var pathBase = _microserviceConfiguration.Configuration["PATH_BASE"];
            if (!string.IsNullOrEmpty(pathBase))
            {
                app.UsePathBase(pathBase);
            }

            app.UseSwagger()
               .UseSwaggerUI(c =>
               {
                   c.SwaggerEndpoint($"{ (!string.IsNullOrEmpty(pathBase) ? pathBase : string.Empty) }/swagger/{_microserviceConfiguration.ServiceName}/swagger.json", _microserviceConfiguration.ServiceName);
                   c.OAuthClientId($"{_microserviceConfiguration.ServiceName.ToLower().Remove(" ")}swaggerui");
                   c.OAuthAppName($"{_microserviceConfiguration.ServiceName} Swagger UI");
               });

            app.UseMiddleware(typeof(RequestCultureMiddleware));
            app.UseMiddleware(typeof(ErrorHandlingMiddleware));

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            app.UseHealthChecks("/hc", new HealthCheckOptions
            {
                Predicate = _ => true,
                ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
            });

            app.UseHealthChecks("/liveness", new HealthCheckOptions
            {
                Predicate = r => r.Name.Contains("self")
            });

            return app;
        }
    }
}
