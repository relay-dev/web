using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using Web.Configuration;
using Web.Rest.Configuration;

namespace Web.Rest
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddRestFramework(this IServiceCollection services, RestConfiguration config, IMvcCoreBuilder mvcBuilder)
        {
            // Add Web Framework
            services.AddWebFramework(config);

            // Add ApiExplorer
            mvcBuilder.AddApiExplorer();

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
            // Use Web Framework
            app.UseWebFramework(configuration, env);

            var pathBase = configuration.Configuration["PATH_BASE"];

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
    }
}
