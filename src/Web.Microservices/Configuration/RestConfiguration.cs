using Web.Configuration;

namespace Web.Rest.Configuration
{
    public class RestConfiguration : WebConfiguration
    {
        public SwaggerConfiguration SwaggerConfiguration { get; set; }
    }
}
