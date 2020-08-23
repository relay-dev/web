using Microservices.Bootstrap;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;

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

            return builder;
        }
    }
}
