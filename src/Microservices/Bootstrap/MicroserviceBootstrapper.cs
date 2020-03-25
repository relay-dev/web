using AutoMapper;
using Core.Caching;
using Core.Data;
using Microservices.Caching;
using Microservices.Filters;
using Microservices.Middleware;
using Microservices.Providers;
using Microservices.Serialization;
using Microservices.Serialization.Impl;
using Core.Plugins.AutoMapper.Data.Resolvers.DatabaseResolver;
using Core.Plugins.Extensions;
using Core.Plugins.SQLServer.Wrappers;
using Core.Providers;
using FluentValidation.AspNetCore;
using HealthChecks.UI.Client;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using NetCore.AutoRegisterDi;
using System.Linq;
using Core.Plugins.Providers;

namespace Microservices.Bootstrap
{
    public class MicroserviceBootstrapper
    {
        private readonly MicroserviceConfiguration _microserviceConfiguration;

        public MicroserviceBootstrapper(MicroserviceConfiguration microserviceConfiguration)
        {
            _microserviceConfiguration = microserviceConfiguration;
        }

        public void ConfigureServices(IServiceCollection services)
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

            services.RegisterAssemblyPublicNonGenericClasses(_microserviceConfiguration.AssembliesToScan)
                .Where(type => type.UnlessAutoWiringOptOut())
                .AsPublicImplementedInterfaces();

            services.AddTransient<Warmup.Warmup>();
            services.AddTransient(typeof(LookupDataKeyResolver<>));
            services.AddTransient(typeof(LookupDataValueResolver<>));
            services.AddTransient<ICacheHelper, DistributedCacheHelper>();
            services.AddTransient<IDatabaseFactory, SQLServerDatabaseFactory>();
            services.AddTransient<IJsonSerializer, NewtonsoftJsonSerializer>();
            services.AddTransient<IConnectionStringProvider, AzureConnectionStringByConfigurationProvider>();
            services.AddTransient<IApplicationContextProvider>(sp => new ApplicationContextProvider(_microserviceConfiguration.ApplicationContext));
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
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

            app.UseHealthChecks("/hc", new HealthCheckOptions()
            {
                Predicate = _ => true,
                ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
            });

            app.UseHealthChecks("/liveness", new HealthCheckOptions
            {
                Predicate = r => r.Name.Contains("self")
            });
        }
    }
}
