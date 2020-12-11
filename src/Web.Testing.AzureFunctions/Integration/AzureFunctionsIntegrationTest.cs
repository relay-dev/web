using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Web.Testing.Integration;
using ExecutionContext = Microsoft.Azure.WebJobs.ExecutionContext;

namespace Web.Testing.AzureFunctions.Integration
{
    public abstract class AzureFunctionsIntegrationTest<TToTest> : AspNetIntegrationTest<TToTest>
    {
        protected ExecutionContext ExecutionContext => new ExecutionContext();

        protected IHostBuilder CreateAzureFunctionsTestHostBuilder<TStartup>() where TStartup : FunctionsStartup, new()
        {
            return new HostBuilder().ConfigureWebJobs(new TStartup().Configure);
        }
    }
}
