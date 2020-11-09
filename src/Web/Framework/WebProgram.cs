using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;

namespace Web.Framework
{
    public class WebProgram<TStartup> where TStartup : class
    {
        protected static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(ConfigureWebHostDefaults)
                .ConfigureAppConfiguration(ConfigureAppConfiguration);

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
                .AddJsonFile($"appsettings.{environment.EnvironmentName}.json", true, true);

            // Load application secrets
            if (IsLocal)
            {
                configBuilder.AddJsonFile("appsettings.Local.json", true, true);
                configBuilder.AddUserSecrets<TStartup>(true);
            }

            configBuilder.AddEnvironmentVariables();
        }

        public static bool IsLocal => bool.Parse(Environment.GetEnvironmentVariable("IS_LOCAL") ?? false.ToString());
    }
}
