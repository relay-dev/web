using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using System;
using Web.Rest.Configuration;

namespace Web.Rest
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddRestFramework(this IServiceCollection services, RestConfiguration config)
        {
            // Add Web Framework
            services.AddWebFramework(config.WebConfiguration);

            // Add ApiExplorer
            //mvcBuilder.AddApiExplorer();

            // Add Health Checks
            services.AddHealthChecks();

            // Add Swagger
            services
                .AddSwaggerGen(options =>
                {
                    options.SwaggerDoc(config.SwaggerConfiguration.Name,
                        new OpenApiInfo
                        {
                            Title = config.SwaggerConfiguration.Title,
                            Version = config.SwaggerConfiguration.Version,
                            Description = config.SwaggerConfiguration.Description
                        });
                });

            // Add Api Versioning
            //services
            //    .AddApiVersioning(cfg =>
            //    {
            //        cfg.DefaultApiVersion = new ApiVersion(config.SwaggerConfiguration.MajorVersion, config.SwaggerConfiguration.MinorVersion);
            //        cfg.AssumeDefaultVersionWhenUnspecified = true;
            //        cfg.ReportApiVersions = true;
            //        cfg.ApiVersionReader = new HeaderApiVersionReader("x-api-version");
            //    });

            return services;
        }

        public static IApplicationBuilder UseRestFramework(this IApplicationBuilder app, RestConfiguration configuration, IWebHostEnvironment env)
        {
            Validate(configuration);

            // Use Web Framework
            app.UseWebFramework(configuration.WebConfiguration, env);

            var pathBase = configuration.WebConfiguration.Configuration["PATH_BASE"];

            app.UseSwagger()
                .UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint($"{ (!string.IsNullOrEmpty(pathBase) ? pathBase : string.Empty) }/swagger/{configuration.SwaggerConfiguration.Name}/swagger.json", configuration.SwaggerConfiguration.Name);
                    //c.OAuthClientId($"{config.SwaggerConfiguration.Name.ToLower()}swaggerui");
                    //c.OAuthAppName($"{config.SwaggerConfiguration.Name} Swagger UI");
                });

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGet("/", async context => context.Response.Redirect("/swagger"));
            });

            return app;
        }

        private static void Validate(RestConfiguration configuration)
        {
            if (configuration == null)
            {
                throw new Exception("RestConfiguration cannot be null");
            }

            if (configuration.SwaggerConfiguration == null)
            {
                throw new Exception("RestConfiguration.SwaggerConfiguration cannot be null");
            }
        }
    }
}
