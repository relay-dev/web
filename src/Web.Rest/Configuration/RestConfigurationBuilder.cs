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

        public RestConfigurationBuilder DocumentUsernameHeaderToken(bool flag = true)
        {
            _restConfiguration.IsDocumentUsernameHeaderToken = flag;

            return this;
        }

        public RestConfiguration Build()
        {
            _restConfiguration.SwaggerConfiguration ??= DefaultSwaggerConfiguration;

            if (!_restConfiguration.IsDocumentUsernameHeaderToken.HasValue)
            {
                _restConfiguration.IsDocumentUsernameHeaderToken = ResolveIsDocumentUsernameHeaderToken();
            }

            return _restConfiguration;
        }

        private bool ResolveIsDocumentUsernameHeaderToken()
        {
            string configSetting = _restConfiguration.ApplicationConfiguration["IsDocumentUsernameHeaderToken"];

            if (!string.IsNullOrEmpty(configSetting) && bool.TryParse(configSetting, out bool isDocumentUsernameHeaderToken))
            {
                return isDocumentUsernameHeaderToken;
            }

            return true;
        }

        private SwaggerConfiguration DefaultSwaggerConfiguration =>
            new SwaggerConfiguration
            {
                Title = _restConfiguration.ApplicationConfiguration["SwaggerConfiguration:Title"] ?? _restConfiguration.ApplicationConfiguration["ServiceName"],
                MajorVersion = Convert.ToInt32(_restConfiguration.ApplicationConfiguration["SwaggerConfiguration:MajorVersion"]),
                MinorVersion = Convert.ToInt32(_restConfiguration.ApplicationConfiguration["SwaggerConfiguration:MinorVersion"]),
                Description = _restConfiguration.ApplicationConfiguration["SwaggerConfiguration:Description"]
            };
    }
}
