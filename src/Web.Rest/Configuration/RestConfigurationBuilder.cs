using System;
using Web.Configuration;

namespace Web.Rest.Configuration
{
    public class RestConfigurationBuilder
    {
        private readonly RestConfiguration _restConfiguration;

        public RestConfigurationBuilder(WebConfigurationBuilder webConfigurationBuilder)
        {
            _restConfiguration = new RestConfiguration(webConfigurationBuilder.Build());
        }

        public RestConfigurationBuilder UseSwaggerConfiguration(SwaggerConfiguration swaggerConfiguration)
        {
            _restConfiguration.SwaggerConfiguration = swaggerConfiguration;

            return this;
        }

        public RestConfiguration Build()
        {
            _restConfiguration.SwaggerConfiguration ??= DefaultSwaggerConfiguration;

            return _restConfiguration;
        }

        private SwaggerConfiguration DefaultSwaggerConfiguration =>
            new SwaggerConfiguration
            {
                Title = _restConfiguration.WebConfiguration.Configuration["SwaggerConfiguration:Title"] ?? _restConfiguration.WebConfiguration.Configuration["ServiceName"],
                MajorVersion = Convert.ToInt32(_restConfiguration.WebConfiguration.Configuration["SwaggerConfiguration:MajorVersion"]),
                MinorVersion = Convert.ToInt32(_restConfiguration.WebConfiguration.Configuration["SwaggerConfiguration:MinorVersion"]),
                Description = _restConfiguration.WebConfiguration.Configuration["SwaggerConfiguration:Description"]
            };
    }
}
