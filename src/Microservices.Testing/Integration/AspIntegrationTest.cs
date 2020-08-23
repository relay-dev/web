using Core.Plugins.NUnit.Integration;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Microservices.Testing.Integration
{
    public abstract class AspIntegrationTest<TToTest> : IntegrationTest<TToTest>
    {
        protected IHostBuilder CreateTestHostBuilder<TStartup>(string basePath = "") where TStartup : class =>
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
                .ConfigureAppConfiguration((webBuilder, configBuilder) =>
                {
                    configBuilder.AddUserSecrets<TStartup>();

                    configBuilder
                        .SetBasePath(basePath)
                        .AddJsonFile("appsettings.json", false, true)
                        .AddJsonFile("appsettings.Development.json", false, true)
                        .AddEnvironmentVariables();
                });
    }
}
