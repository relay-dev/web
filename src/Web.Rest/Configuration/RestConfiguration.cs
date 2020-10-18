using Microsoft.Extensions.Configuration;
using Web.Configuration;

namespace Web.Rest.Configuration
{
    public class RestConfiguration
    {
        public RestConfiguration() { }

        public RestConfiguration(WebConfiguration webConfiguration)
        {
            WebConfiguration = webConfiguration;
        }

        public RestConfiguration(WebConfiguration webConfiguration, SwaggerConfiguration swaggerConfiguration)
        {
            WebConfiguration = webConfiguration;
            SwaggerConfiguration = swaggerConfiguration;
        }

        private WebConfiguration _webConfiguration;
        public WebConfiguration WebConfiguration
        {
            get => _webConfiguration;
            set
            {
                _webConfiguration = value;
                _webConfiguration.IsAddApiExplorer = true;
            }
        }

        public bool IsDocumentUsernameHeaderToken { get; set; }
        public SwaggerConfiguration SwaggerConfiguration { get; set; }
        public IConfiguration ApplicationConfiguration => WebConfiguration.ApplicationConfiguration;
    }
}
