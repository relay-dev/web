using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Web.Testing.Integration;

namespace Web.Testing.AzureFunctions.Integration
{
    public abstract class AzureFunctionsIntegrationTest<TToTest> : WebIntegrationTest<TToTest>
    {
        public IHost Bootstrap<TStartup>() where TStartup : FunctionsStartup, new()
        {
            return new HostBuilder()
                .ConfigureWebJobs(new TStartup().Configure)
                .Build();
        }
    }
}
