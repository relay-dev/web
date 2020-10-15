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

        public AzureFunctionsConfiguration Build()
        {
            return _azureFunctionsConfiguration;
        }
    }
}
