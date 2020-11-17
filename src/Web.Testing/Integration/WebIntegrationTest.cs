using Core.Plugins.NUnit.Integration;
using Core.Providers;
using Extensions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using ILogger = Microsoft.Extensions.Logging.ILogger;

namespace Web.Testing.Integration
{
    public abstract class WebIntegrationTest<TToTest> : IntegrationTest<TToTest>
    {
        protected ILogger Logger => ResolveService<ILogger<TToTest>>();

        protected override void BootstrapTest()
        {
            base.OneTimeSetUp();

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
                    ConfigureIntegrationTestServices(services);
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

        protected virtual IServiceCollection ConfigureIntegrationTestServices(IServiceCollection services)
        {
            return services;
        }

        protected HttpRequest CreateHttpRequest()
        {
            var httpRequest = new DefaultHttpContext().Request;

            httpRequest.Headers["X-Username"] = TestUsername;

            return httpRequest;
        }

        protected HttpRequest CreateHttpRequest(string key, string val)
        {
            HttpRequest request = CreateHttpRequest();

            request.QueryString = QueryString.Create(
                new Dictionary<string, string>
                {
                    {key, val}
                });

            return request;
        }

        protected HttpRequest CreateHttpRequest(Dictionary<string, string> queryStringParameters)
        {
            HttpRequest request = CreateHttpRequest();

            request.QueryString = QueryString.Create(queryStringParameters);

            return request;
        }

        protected void RegisterControllers<TStartup>(IServiceCollection services)
        {
            foreach (Type controllerType in typeof(TStartup).Assembly.GetTypes().Where(t => t.Name.EndsWith("Controller")))
            {
                services.AddScoped(controllerType);
            }
        }

        private string GetAssemblyDirectory(Assembly assembly)
        {
            string path = Uri.UnescapeDataString(new UriBuilder(assembly.CodeBase).Path);

            return Path.GetDirectoryName(path);
        }
    }
}
