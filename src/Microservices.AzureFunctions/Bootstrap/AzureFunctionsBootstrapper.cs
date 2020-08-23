using Microservices.AzureFunctions.Configuration;
using Microservices.Bootstrap;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;

namespace Microservices.AzureFunctions.Bootstrap
{
    public class AzureFunctionsBootstrapper
    {
        private readonly AzureFunctionsConfiguration _azureFunctionsConfiguration;
        
        public AzureFunctionsBootstrapper(AzureFunctionsConfiguration azureFunctionsConfiguration)
        {
            _azureFunctionsConfiguration = azureFunctionsConfiguration;
        }

        public IFunctionsHostBuilder Configure(IFunctionsHostBuilder builder)
        {
            new MicroserviceBootstrapper(_azureFunctionsConfiguration)
                .ConfigureCommonServices(builder.Services);

            // Add the Azure Functions configuration
            builder.Services.AddSingleton(_azureFunctionsConfiguration);

            return builder;
        }
    }
}
