using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using System;
using Web.Rest.Configuration;

namespace Web.Rest
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddRestFramework(this IServiceCollection services, RestConfiguration restConfiguration)
        {
            // Add Web Framework
            services.AddWebFramework(restConfiguration.WebConfiguration);

            // Add Health Checks
            services.AddHealthChecks();

            // Add Swagger
            services
                .AddSwaggerGen(options =>
                {
                    options.SwaggerDoc(restConfiguration.SwaggerConfiguration.Name,
                        new OpenApiInfo
                        {
                            Title = restConfiguration.SwaggerConfiguration.Title,
                            Version = restConfiguration.SwaggerConfiguration.Version,
                            Description = restConfiguration.SwaggerConfiguration.Description
                        });
                });

            // Add Api Versioning
            //services
            //    .AddApiVersioning(cfg =>
            //    {
            //        cfg.DefaultApiVersion = new ApiVersion(restConfiguration.SwaggerConfiguration.MajorVersion, restConfiguration.SwaggerConfiguration.MinorVersion);
            //        cfg.AssumeDefaultVersionWhenUnspecified = true;
            //        cfg.ReportApiVersions = true;
            //        cfg.ApiVersionReader = new HeaderApiVersionReader("x-api-version");
            //    });

            // Add RestConfiguration
            services.AddSingleton(restConfiguration);

            return services;
        }

        public static IApplicationBuilder UseRestFramework(this IApplicationBuilder app, RestConfiguration restConfiguration, IWebHostEnvironment env)
        {
            Validate(restConfiguration);

            // Use Web Framework
            app.UseWebFramework(restConfiguration.WebConfiguration, env);

            var pathBase = restConfiguration.ApplicationConfiguration["PATH_BASE"];

            app.UseSwagger()
                .UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint($"{ (!string.IsNullOrEmpty(pathBase) ? pathBase : string.Empty) }/swagger/{restConfiguration.SwaggerConfiguration.Name}/swagger.json", restConfiguration.SwaggerConfiguration.Name);
                    //c.OAuthClientId($"{restConfiguration.SwaggerConfiguration.Name.ToLower()}swaggerui");
                    //c.OAuthAppName($"{restConfiguration.SwaggerConfiguration.Name} Swagger UI");
                });

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGet("/", async context => context.Response.Redirect("/swagger"));
            });

            return app;
        }

        public static IServiceCollection AddRestFramework<TDbContext>(this IServiceCollection services, RestConfiguration restConfiguration) where TDbContext : DbContext
        {
            services = AddRestFramework(services, restConfiguration);

            services.AddDbContext<TDbContext>();

            return services;
        }

        private static void Validate(RestConfiguration restConfiguration)
        {
            if (restConfiguration == null)
            {
                throw new Exception("RestConfiguration cannot be null");
            }

            if (restConfiguration.SwaggerConfiguration == null)
            {
                throw new Exception("RestConfiguration.SwaggerConfiguration cannot be null");
            }
        }
    }
}
