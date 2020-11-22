using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Web.AzureFunctions;
using Web.AzureFunctions.Configuration;
using Web.AzureFunctions.Extensions;
using Web.Samples.OrderManagement.Domain.Commands.Get;
using Web.Samples.OrderManagement.Domain.Context;
using Web.Samples.OrderManagement.EventHandlers;

[assembly: FunctionsStartup(typeof(Startup))]
namespace Web.Samples.OrderManagement.EventHandlers
{
    public class Startup : FunctionsStartup
    {
        private readonly AzureFunctionsConfiguration _azureFunctionsConfiguration;

        public Startup()
        {
            _azureFunctionsConfiguration = BuildAzureFunctionsConfiguration();
        }

        public override void Configure(IFunctionsHostBuilder builder)
        {
            builder.Services.AddAzureFunctionsFramework<OrderContext>(_azureFunctionsConfiguration);
        }

        private AzureFunctionsConfiguration BuildAzureFunctionsConfiguration()
        {
            return new ConfigurationBuilder()
                .AsAzureFunctionsConfiguration()
                .UseApplicationName(GetType().AssemblyQualifiedName)
                .UseFunctionsFromAssemblyContaining<Startup>()
                .UseCommandHandlersFromAssemblyContaining<GetOrderByIdHandler>()
                .AsEventHandler()
                .Build();
        }
    }
}
