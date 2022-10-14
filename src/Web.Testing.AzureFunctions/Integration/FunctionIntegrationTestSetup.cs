using Core.Plugins.NUnit;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;

namespace Web.Testing.AzureFunctions.Integration
{
    /// <summary>
    /// Functionality to support anything needed to setup an Azure Function test run session
    /// </summary>
    public class FunctionIntegrationTestSetup : TestSetupBase
    {
        /// <summary>
        /// Creates a host builder using configuration from Startup.cs for function tests to run against
        /// </summary>
        public virtual IHostBuilder CreateFunctionsTestHostBuilder<TStartup>() where TStartup : FunctionsStartup, new()
        {
            return new HostBuilder()
                .ConfigureWebHostDefaults(builder =>
                {
                    builder.ConfigureLogging((hostingContext, logging) =>
                    {
                        logging.AddConsole();
                        logging.AddDebug();
                    });
                })
                .ConfigureWebJobs(builder =>
                {
                    new TStartup().Configure(builder);
                })
                .ConfigureServices((ctx, services) =>
                {
                    ConfigureApplicationServices(services);
                })
                .ConfigureAppConfiguration((ctx, configBuilder) =>
                {
                    configBuilder
                        .AddJsonFile("appsettings.json", true, true)
                        .AddJsonFile("appsettings.Development.json", true, true)
                        .AddJsonFile("local.settings.json", true, true)
                        .AddUserSecrets<TStartup>(true)
                        .AddEnvironmentVariables();

                    var settings = GetLocalSettings<TStartup>();

                    if (settings != null && settings.Values != null)
                    {
                        foreach (var setting in settings.Values)
                        {
                            Environment.SetEnvironmentVariable(setting.Key, setting.Value);
                        }
                    }
                });
        }
    }
}
