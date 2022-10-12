using Core.Plugins.NUnit;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;

namespace Web.Testing.Integration
{
    /// <summary>
    /// Functionality to support anything needed to setup a web application test run session
    /// </summary>
    public class WebIntegrationTestSetup : TestSetupBase
    {
        /// <summary>
        /// Creates a host builder using configuration from Startup.cs for web tests to run against
        /// </summary>
        public virtual IHostBuilder CreateWebTestHostBuilder<TStartup>() where TStartup : class =>
            Host.CreateDefaultBuilder(new string[0])
                .ConfigureWebHostDefaults(builder =>
                {
                    builder.UseStartup<TStartup>();
                    builder.ConfigureLogging((hostingContext, logging) =>
                    {
                        logging.AddConsole();
                        logging.AddDebug();
                    });
                })
                .ConfigureServices((ctx, services) =>
                {
                    RegisterControllers<TStartup>(services);
                    ConfigureApplicationServices(services);
                })
                .ConfigureAppConfiguration((webBuilder, configBuilder) =>
                {
                    configBuilder
                        .AddJsonFile("appsettings.json", true, true)
                        .AddJsonFile("appsettings.Development.json", true, true)
                        .AddJsonFile("local.settings.json", true, true)
                        .AddUserSecrets<TStartup>(true)
                        .AddEnvironmentVariables();

                    foreach (var setting in GetLocalSettings<TStartup>().Values)
                    {
                        Environment.SetEnvironmentVariable(setting.Key, setting.Value);
                    }
                });

        protected virtual void RegisterControllers<TStartup>(IServiceCollection services)
        {
            foreach (Type controllerType in typeof(TStartup).Assembly.GetTypes().Where(t => t.Name.EndsWith("Controller")))
            {
                services.AddScoped(controllerType);
            }
        }
    }
}
