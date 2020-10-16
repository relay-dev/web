using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;

namespace Web.Rest.Framework
{
    public class RestProgram<TStartup> where TStartup : class
    {
        protected static void ConfigureWebHostDefaults(IWebHostBuilder webBuilder)
        {
            webBuilder.UseStartup<TStartup>();
            webBuilder.ConfigureLogging((hostingContext, logging) =>
            {
                logging.AddConfiguration(hostingContext.Configuration.GetSection("Logging"));

                if (IsLocal)
                {
                    logging.AddConsole();
                    logging.AddDebug();
                }
                else
                {
                    logging.AddApplicationInsights();
                    logging.AddAzureWebAppDiagnostics();
                }
            });
        }

        protected static void ConfigureAppConfiguration(HostBuilderContext webBuilder, IConfigurationBuilder configBuilder)
        {
            IHostEnvironment environment = webBuilder.HostingEnvironment;

            // Load the appsettings file and overlay environment specific configurations
            configBuilder
                .AddJsonFile("appsettings.json", false, true)
                .AddJsonFile($"appsettings.{environment.EnvironmentName}.json", true, true)
                .AddJsonFile("local.settings.json", true, true)
                .AddEnvironmentVariables();

            // Load application secrets
            if (IsLocal)
            {
                configBuilder.AddUserSecrets<TStartup>();
            }
        }

        private static bool IsLocal => bool.Parse(Environment.GetEnvironmentVariable("IS_LOCAL") ?? false.ToString());
    }
}
