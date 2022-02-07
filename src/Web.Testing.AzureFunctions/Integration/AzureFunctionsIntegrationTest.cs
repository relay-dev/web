using Microsoft.AspNetCore.Hosting;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Web.Testing.Integration;
using ExecutionContext = Microsoft.Azure.WebJobs.ExecutionContext;

namespace Web.Testing.AzureFunctions.Integration
{
    public abstract class AzureFunctionsIntegrationTest<TToTest> : AspNetIntegrationTest<TToTest>
    {
        protected ExecutionContext ExecutionContext => new ExecutionContext();

        protected IHostBuilder CreateAzureFunctionsTestHostBuilder<TStartup>() where TStartup : FunctionsStartup, new()
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
                        .AddJsonFile("C:\\Azure\\appsettings.KeyVault.json", true, true)
                        .AddJsonFile("appsettings.Local.json", true, true)
                        .AddJsonFile("local.settings.json", true, true)
                        .AddUserSecrets<TStartup>(true)
                        .AddEnvironmentVariables();
                });
        }

        protected virtual void ConfigureApplicationServices(IServiceCollection services) { }
    }
}
