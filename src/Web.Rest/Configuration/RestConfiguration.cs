using Web.Configuration;

namespace Web.Rest.Configuration
{
    public class RestConfiguration
    {
        public RestConfiguration(WebConfiguration webConfiguration)
        {
            WebConfiguration = webConfiguration;
        }

        public WebConfiguration WebConfiguration { get; set; }
        public SwaggerConfiguration SwaggerConfiguration { get; set; }
    }
}
