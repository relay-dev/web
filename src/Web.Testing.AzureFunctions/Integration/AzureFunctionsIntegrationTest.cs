using Microsoft.AspNetCore.Hosting;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using Web.Testing.Integration;
using ExecutionContext = Microsoft.Azure.WebJobs.ExecutionContext;

namespace Web.Testing.AzureFunctions.Integration
{
    public abstract class AzureFunctionsIntegrationTest : AspNetIntegrationTest
    {
        protected virtual IHostBuilder CreateAzureFunctionsTestHostBuilder<TStartup>() where TStartup : FunctionsStartup, new()
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

        protected virtual ExecutionContext ExecutionContext => new ExecutionContext();
    }

    public abstract class AzureFunctionsIntegrationTest<TSUT> : AzureFunctionsIntegrationTest
    {
        protected virtual TSUT SUT => (TSUT)CurrentTestProperties.Get(SutKey);
        protected override ILogger Logger => ResolveService<ILogger<TSUT>>();

        protected override void BootstrapTest()
        {
            base.BootstrapTest();

            var serviceProvider = (IServiceProvider)CurrentTestProperties.Get(ServiceProviderKey);

            TSUT sut = serviceProvider.GetRequiredService<TSUT>();

            CurrentTestProperties.Set(SutKey, sut);
        }

        protected const string SutKey = "_sut";
    }
}
