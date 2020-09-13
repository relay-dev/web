using Web.Configuration;

namespace Web.AzureFunctions.Configuration
{
    public class AzureFunctionsConfiguration
    {
        public AzureFunctionsConfiguration() { }

        public AzureFunctionsConfiguration(WebConfiguration webConfiguration)
        {
            WebConfiguration = webConfiguration;
        }

        public WebConfiguration WebConfiguration { get; set; }
    }
}
