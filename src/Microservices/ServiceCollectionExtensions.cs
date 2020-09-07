using AutoMapper;
using Core.Application;
using Core.Caching;
using Core.Plugins.Application;
using Core.Plugins.AutoMapper.Resolvers.Database;
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
using MediatR;
using Microservices.Caching;
using Microservices.Configuration;
using Microservices.Data;
using Microservices.Data.Impl;
using Microservices.Mappers;
using Microservices.Middleware;
using Microservices.Serialization;
using Microservices.Serialization.Impl;
using Microservices.Warmup;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Azure.EventGrid;
using Microsoft.Azure.EventGrid.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Microservices
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddWebFramework(this IServiceCollection services, MicroserviceConfiguration configuration)
        {
            // Add MVC and Newtonsoft
            IMvcCoreBuilder mvcBuilder = services
                .AddMvcCore()
                .AddApiExplorer()
                .AddNewtonsoftJson(options => options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore);

            // Add AutoMapper
            if (configuration.MapperTypes.Any())
            {
                // If the consumer is using AutoMapper, then add common mappers
                configuration.MapperTypes.Add(typeof(PrimitiveMappers));
                configuration.MapperTypes.Add(typeof(SystemMappers));

                services
                    .AddAutoMapper(cfg =>
                    {
                        services.AddSingleton(provider =>
                        {
                            cfg.ConstructServicesUsing(type => ActivatorUtilities.CreateInstance(provider, type));
                            return cfg;
                        });
                    }, configuration.MapperTypes.ToArray());
            }

            // Add Fluent Validators
            if (configuration.ValidatorTypes.Any())
            {
                mvcBuilder.AddFluentValidation();

                configuration.ValidatorTypes.ForEach(v => services.AddTransient(v.Key, v.Value));
            }

            // Add MediatR
            if (configuration.CommandHandlerTypes.Any())
            {
                services.AddMediatR(configuration.CommandHandlerTypes.ToArray());
            }

            // Add Storage Account client
            if (configuration.Configuration.GetConnectionString("DefaultStorageConnection") != null)
            {
                services.AddTransient<IStorageAccount>(sp => new AzureStorageAccount(sp.GetService<IConnectionStringProvider>().Get("DefaultStorageConnection")));
            }

            // Add Event Grid client
            if (configuration.Configuration.GetConnectionString("DefaultEventGridConnection") != null)
            {
                services.AddScoped<IEventGridClient>(sp =>
                {
                    var connectionStringParser = sp.GetRequiredService<IConnectionStringParser>();
                    var connectionStringProvider = sp.GetRequiredService<IConnectionStringProvider>();
                    var connectionStringSegments = connectionStringParser.Parse(connectionStringProvider.Get("DefaultEventGridConnection"));

                    return new EventGridClient(new TopicCredentials(connectionStringSegments.ToDynamic().Key));
                });
            }

            // Add Warmup
            if (configuration.WarmupTypes.Any())
            {
                services.AddTransient<WarmupTaskExecutor>();

                configuration.WarmupTypes.ForEach(warmupType =>
                {
                    services.AddTransient(warmupType);
                });
            }

            // Add Caching
            services.AddDistributedMemoryCache();

            // Add the microservice configuration and context
            services.AddSingleton(configuration);
            services.AddSingleton(configuration.ApplicationContext);

            // Add common utilities
            services.AddScoped(typeof(LookupDataKeyResolver<>));
            services.AddScoped(typeof(LookupDataValueResolver<>));
            services.AddScoped<IInlineValidator, InlineValidator>();
            services.AddScoped<IGlobalHelper, GlobalHelperWrapper>();
            services.AddScoped<ICacheHelper, DistributedCacheHelper>();
            services.AddScoped<IUsernameProvider, UsernameProvider>();
            services.AddScoped<IDateTimeProvider, DateTimeUtcProvider>();
            services.AddScoped<IRandomCodeProvider, RandomCodeProvider>();
            services.AddScoped<IRandomLongProvider, RandomLongProvider>();
            services.AddScoped<IEntityAuditor, EntityFrameworkEntityAuditor>();
            services.AddScoped<ICommandContextProvider, CommandContextProvider>();
            services.AddScoped<IConnectionStringParser, ConnectionStringParser>();
            services.AddScoped<IStorageAccountFactory, AzureStorageAccountFactory>();
            services.AddScoped<IApplicationContextProvider, ApplicationContextProvider>();
            services.AddScoped<IConnectionStringProvider, AzureConnectionStringByConfigurationProvider>();
            services.AddSingleton<IApplicationContextProvider>(sp => new ApplicationContextProvider(configuration.ApplicationContext));
            services.AddTransient<IJsonSerializer, SystemJsonSerializer>();

            return services;
        }

        public static IServiceCollection AddMicroserviceFramework(this IServiceCollection services, MicroserviceConfiguration configuration)
        {
            // Configure services common to both Microservices and Azure Functions
            services.AddWebFramework(configuration);

            // Add Health Checks
            services.AddHealthChecks();

            // Add Swagger
            services
                .AddSwaggerGen(options =>
                {
                    options.SwaggerDoc(configuration.SwaggerConfiguration.Name,
                        new OpenApiInfo
                        {
                            Title = configuration.SwaggerConfiguration.Title,
                            Version = configuration.SwaggerConfiguration.Version,
                            Description = configuration.SwaggerConfiguration.Description
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

            return services;
        }

        public static IApplicationBuilder UseMicroserviceFramework(this IApplicationBuilder app, MicroserviceConfiguration configuration, IWebHostEnvironment env)
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

            app.UseSwagger()
                .UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint($"{ (!string.IsNullOrEmpty(pathBase) ? pathBase : string.Empty) }/swagger/{configuration.SwaggerConfiguration.Name}/swagger.json", configuration.SwaggerConfiguration.Name);
                    //c.OAuthClientId($"{_microserviceConfiguration.SwaggerConfiguration.Name.ToLower()}swaggerui");
                    //c.OAuthAppName($"{_microserviceConfiguration.SwaggerConfiguration.Name} Swagger UI");
                });

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

        private static bool IsLocal => bool.Parse(Environment.GetEnvironmentVariable("IS_LOCAL") ?? false.ToString());
    }
}
