using Web.Configuration;

namespace Web.AzureFunctions.Configuration
{
    public class AzureFunctionsConfigurationBuilder
    {
        private readonly AzureFunctionsConfiguration _azureFunctionsConfiguration;

        public AzureFunctionsConfigurationBuilder(WebConfigurationBuilder webConfigurationBuilder)
        {
            _azureFunctionsConfiguration = new AzureFunctionsConfiguration(webConfigurationBuilder.Build());
        }

        public AzureFunctionsConfigurationBuilder AsEventHandler()
        {
            _azureFunctionsConfiguration.IsEventHandler = true;

            return this;
        }

        public AzureFunctionsConfiguration Build()
        {
            return _azureFunctionsConfiguration;
        }
    }
}
