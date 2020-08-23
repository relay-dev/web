using AutoMapper;
using Core.Caching;
using Core.Plugins.Application;
using Core.Plugins.AutoMapper.Data.Resolvers.DatabaseResolver;
using Core.Plugins.Extensions;
using Core.Plugins.Microsoft.Azure.Storage;
using Core.Plugins.Microsoft.Azure.Storage.Impl;
using Core.Plugins.Microsoft.Azure.Wrappers;
using Core.Plugins.Providers;
using Core.Plugins.Utilities;
using Core.Plugins.Validation;
using Core.Providers;
using Core.Utilities;
using FluentValidation.AspNetCore;
using HealthChecks.UI.Client;
using MediatR;
using Microservices.Caching;
using Microservices.Configuration;
using Microservices.Middleware;
using Microservices.Serialization;
using Microservices.Serialization.Impl;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Azure.EventGrid;
using Microsoft.Azure.EventGrid.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using System.Linq;

namespace Microservices.Bootstrap
{
    public class MicroserviceBootstrapper
    {
        private readonly MicroserviceConfiguration _microserviceConfiguration;

        public MicroserviceBootstrapper(MicroserviceConfiguration microserviceConfiguration)
        {
            _microserviceConfiguration = microserviceConfiguration;
        }

        public IServiceCollection ConfigureServices(IServiceCollection services)
        {
            // Configure services common to both Microservices and Azure Functions
            ConfigureCommonServices(services);

            // Add Health Checks
            services.AddHealthChecks();

            // Add Swagger
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

            // Add Api Versioning
            //services
            //    .AddApiVersioning(cfg =>
            //    {
            //        cfg.DefaultApiVersion = new ApiVersion(_microserviceConfiguration.SwaggerConfiguration.MajorVersion, _microserviceConfiguration.SwaggerConfiguration.MinorVersion);
            //        cfg.AssumeDefaultVersionWhenUnspecified = true;
            //        cfg.ReportApiVersions = true;
            //        cfg.ApiVersionReader = new HeaderApiVersionReader("x-api-version");
            //    });

            // Add the microservice configuration
            services.AddSingleton(_microserviceConfiguration);

            return services;
        }

        public IServiceCollection ConfigureCommonServices(IServiceCollection services)
        {
            // Add MVC and Newtonsoft
            IMvcCoreBuilder mvcBuilder = services
                .AddMvcCore()
                .AddNewtonsoftJson(options => options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore);

            // Add AutoMapper
            if (_microserviceConfiguration.MapperTypes.Any())
            {
                services
                    .AddAutoMapper(cfg =>
                    {
                        services.AddSingleton(provider =>
                        {
                            cfg.ConstructServicesUsing(type => ActivatorUtilities.CreateInstance(provider, type));
                            return cfg;
                        });
                    }, _microserviceConfiguration.MapperTypes.ToArray());
            }

            // Add Fluent Validators
            if (_microserviceConfiguration.ValidatorTypes.Any())
            {
                mvcBuilder.AddFluentValidation();

                _microserviceConfiguration.ValidatorTypes.ForEach(v => services.AddTransient(v.Key, v.Value));
            }

            // Add MediatR
            if (_microserviceConfiguration.CommandHandlerTypes.Any())
            {
                services.AddMediatR(_microserviceConfiguration.CommandHandlerTypes.ToArray());
            }

            // Add Storage Account client
            if (_microserviceConfiguration.Configuration.GetConnectionString("DefaultStorageConnection") != null)
            {
                services.AddTransient<IStorageAccount>(sp => new AzureStorageAccount(sp.GetService<IConnectionStringProvider>().Get("DefaultStorageConnection")));
            }

            // Add Event Grid client
            if (_microserviceConfiguration.Configuration.GetConnectionString("DefaultEventGridConnection") != null)
            {
                services.AddScoped<EventGridSubscriber>();
                services.AddScoped<IEventGridClient>(sp =>
                {
                    var parser = new ConnectionStringParser(sp.GetRequiredService<IConnectionStringProvider>().Get("DefaultEventGridConnection"));

                    return new EventGridClient(new TopicCredentials(parser.Segment.Key));
                });
            }

            // Add Caching
            services.AddDistributedMemoryCache();

            // Add other common utilities
            services.AddSingleton<Warmup.Warmup>();
            services.AddScoped(typeof(LookupDataKeyResolver<>));
            services.AddScoped(typeof(LookupDataValueResolver<>));
            services.AddScoped<ICacheHelper, DistributedCacheHelper>();
            services.AddScoped<IGlobalHelper, GlobalHelperWrapper>();
            services.AddScoped<IInlineValidator, InlineValidator>();
            services.AddScoped<IStorageAccountFactory, AzureStorageAccountFactory>();
            services.AddScoped<IConnectionStringProvider, AzureConnectionStringByConfigurationProvider>();
            services.AddTransient<IJsonSerializer, SystemJsonSerializer>();
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
