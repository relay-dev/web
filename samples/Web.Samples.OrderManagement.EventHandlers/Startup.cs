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
        public override void Configure(IFunctionsHostBuilder builder)
        {
            var azureFunctionsConfiguration = BuildAzureFunctionsConfiguration(builder);

            builder.Services.AddAzureFunctionsFramework<OrderContext>(azureFunctionsConfiguration);
        }

        private AzureFunctionsConfiguration BuildAzureFunctionsConfiguration(IFunctionsHostBuilder builder)
        {
            return new ConfigurationBuilder()
                .AsAzureFunctionsConfiguration()
                .UseApplicationName("OrderManagement.EventHandlers")
                .UseConfiguration(builder.GetContext().Configuration)
                .UseFunctionsFromAssemblyContaining<Startup>()
                .UseCommandHandlersFromAssemblyContaining<GetOrderByIdHandler>()
                .AsEventHandler()
                .Build();
        }
    }
}
