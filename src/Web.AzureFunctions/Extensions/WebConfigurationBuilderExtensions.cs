using Web.AzureFunctions.Configuration;
using Web.Configuration;

namespace Web.AzureFunctions.Extensions
{
    public static class WebConfigurationBuilderExtensions
    {
        public static AzureFunctionsConfigurationBuilder AsAzureFunctionsConfiguration(this WebConfigurationBuilder webConfigurationBuilder)
        {
            return new AzureFunctionsConfigurationBuilder(webConfigurationBuilder);
        }
    }
}
