using Web.Configuration;

namespace Web.Rest.Configuration
{
    public class RestConfiguration : WebConfiguration
    {
        public bool? IsDocumentUsernameHeaderToken { get; set; }
        public SwaggerConfiguration SwaggerConfiguration { get; set; }
    }
}
