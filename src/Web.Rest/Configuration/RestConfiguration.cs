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

        public WebConfiguration WebConfiguration { get; set; }
        public SwaggerConfiguration SwaggerConfiguration { get; set; }
    }
}
