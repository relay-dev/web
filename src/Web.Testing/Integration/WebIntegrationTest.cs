﻿using Core.Providers;
using Extensions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Linq;

namespace Web.Testing.Integration
{
    public abstract class WebIntegrationTest<TToTest> : AspNetIntegrationTest<TToTest>
    {
        protected override void BootstrapTest()
        {
            base.BootstrapTest();

            IUsernameProvider usernameProvider = ResolveService<IUsernameProvider>();

            usernameProvider.Set(TestUsername);
        }

        protected IHostBuilder CreateTestHostBuilder<TStartup>(string basePath = null) where TStartup : class =>
            Microsoft.Extensions.Hosting.Host.CreateDefaultBuilder(new string[0])
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<TStartup>();
                    webBuilder.ConfigureLogging((hostingContext, logging) =>
                    {
                        logging.AddConsole();
                        logging.AddDebug();
                    });
                })
                .ConfigureServices((webBuilder, services) =>
                {
                    RegisterControllers<TStartup>(services);
                    ConfigureTestServices(services);
                })
                .ConfigureAppConfiguration((webBuilder, configBuilder) =>
                {
                    basePath ??= Path.Combine(AppDomain.CurrentDomain.BaseDirectory.SubstringBefore("tests"), "src", typeof(TStartup).Namespace);

                    configBuilder
                        .SetBasePath(basePath)
                        .AddJsonFile("appsettings.json", true, true)
                        .AddJsonFile("appsettings.Development.json", true, true)
                        .AddJsonFile("appsettings.Local.json", true, true)
                        .AddJsonFile("local.settings.json", true, true)
                        .AddUserSecrets<TStartup>(true)
                        .AddEnvironmentVariables();
                });

        protected virtual IServiceCollection ConfigureTestServices(IServiceCollection services)
        {
            return services;
        }

        protected void RegisterControllers<TStartup>(IServiceCollection services)
        {
            foreach (Type controllerType in typeof(TStartup).Assembly.GetTypes().Where(t => t.Name.EndsWith("Controller")))
            {
                services.AddScoped(controllerType);
            }
        }
    }
}
