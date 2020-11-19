using Microsoft.Extensions.Configuration;
using System;
using Web.Configuration;

namespace Web.Rest.Configuration
{
    public class RestConfigurationBuilder<TConfiguration> : WebConfigurationBuilder<TConfiguration> where TConfiguration : class
    {
        private readonly RestConfigurationBuilderContainer _container;
        private readonly IConfiguration _configuration;

        public RestConfigurationBuilder(IConfiguration configuration)
        {
            _configuration = configuration;
            _container = new RestConfigurationBuilderContainer();
        }

        public RestConfigurationBuilder<TConfiguration> UseSwaggerConfiguration(SwaggerConfiguration swaggerConfiguration)
        {
            _container.SwaggerConfiguration = swaggerConfiguration;

            return this;
        }

        public RestConfigurationBuilder<TConfiguration> DocumentUsernameHeaderToken(bool flag = true)
        {
            _container.IsDocumentUsernameHeaderToken = flag;

            return this;
        }

        public override TConfiguration Build()
        {
            var restConfiguration = base.Build() as RestConfiguration;

            if (restConfiguration == null)
            {
                throw new InvalidOperationException("restConfiguration cannot be null");
            }

            restConfiguration.IsDocumentUsernameHeaderToken = _container.IsDocumentUsernameHeaderToken;
            restConfiguration.SwaggerConfiguration = _container.SwaggerConfiguration;

            if (restConfiguration.IsDocumentUsernameHeaderToken == null)
            {
                restConfiguration.IsDocumentUsernameHeaderToken = ResolveIsDocumentUsernameHeaderToken();
            }

            if (restConfiguration.SwaggerConfiguration == null)
            {
                restConfiguration.SwaggerConfiguration = DefaultSwaggerConfiguration;
            }

            return restConfiguration as TConfiguration;
        }

        private bool ResolveIsDocumentUsernameHeaderToken()
        {
            string configSetting = _configuration["IsDocumentUsernameHeaderToken"];

            if (!string.IsNullOrEmpty(configSetting) && bool.TryParse(configSetting, out bool isDocumentUsernameHeaderToken))
            {
                return isDocumentUsernameHeaderToken;
            }

            return true;
        }

        private SwaggerConfiguration DefaultSwaggerConfiguration =>
            new SwaggerConfiguration
            {
                Title = _configuration["SwaggerConfiguration:Title"] ?? _configuration["ServiceName"],
                MajorVersion = Convert.ToInt32(_configuration["SwaggerConfiguration:MajorVersion"]),
                MinorVersion = Convert.ToInt32(_configuration["SwaggerConfiguration:MinorVersion"]),
                Description = _configuration["SwaggerConfiguration:Description"]
            };

        internal class RestConfigurationBuilderContainer : RestConfiguration { }
    }
}
