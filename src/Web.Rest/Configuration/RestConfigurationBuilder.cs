using Microsoft.Extensions.Configuration;
using System;
using Web.Configuration;

namespace Web.Rest.Configuration
{
    public class RestConfigurationBuilder : RestConfigurationBuilder<RestConfigurationBuilder, RestConfiguration>
    {
        
    }

    public class RestConfigurationBuilder<TBuilder, TResult> : WebConfigurationBuilder<TBuilder, TResult> where TBuilder : class where TResult : class
    {
        private readonly RestConfigurationBuilderContainer _container;

        public RestConfigurationBuilder()
        {
            _container = new RestConfigurationBuilderContainer();
        }

        public TBuilder UseSwaggerConfiguration(SwaggerConfiguration swaggerConfiguration)
        {
            _container.SwaggerConfiguration = swaggerConfiguration;

            return this as TBuilder;
        }

        public TBuilder DocumentUsernameHeaderToken(bool flag = true)
        {
            _container.IsDocumentUsernameHeaderToken = flag;

            return this as TBuilder;
        }

        public override TResult Build()
        {
            var restConfiguration = new RestConfiguration();

            return BuildUsing(restConfiguration);
        }

        protected override TResult BuildUsing<TConfiguration>(TConfiguration configuration)
        {
            var restConfiguration = configuration as RestConfiguration;

            if (restConfiguration == null)
            {
                throw new InvalidOperationException("restConfiguration cannot be null");
            }

            base.BuildUsing(restConfiguration);

            restConfiguration.IsDocumentUsernameHeaderToken = _container.IsDocumentUsernameHeaderToken;
            restConfiguration.SwaggerConfiguration = _container.SwaggerConfiguration;

            if (restConfiguration.IsDocumentUsernameHeaderToken == null)
            {
                restConfiguration.IsDocumentUsernameHeaderToken = ResolveIsDocumentUsernameHeaderToken(restConfiguration.Configuration);
            }

            if (restConfiguration.SwaggerConfiguration == null)
            {
                restConfiguration.SwaggerConfiguration = GetDefaultSwaggerConfiguration(restConfiguration.Configuration);
            }

            return restConfiguration as TResult;
        }

        private bool ResolveIsDocumentUsernameHeaderToken(IConfiguration configuration)
        {
            string configSetting = configuration["IsDocumentUsernameHeaderToken"];

            if (!string.IsNullOrEmpty(configSetting) && bool.TryParse(configSetting, out bool isDocumentUsernameHeaderToken))
            {
                return isDocumentUsernameHeaderToken;
            }

            return true;
        }

        private SwaggerConfiguration GetDefaultSwaggerConfiguration(IConfiguration configuration) =>
            new SwaggerConfiguration
            {
                Title = configuration["SwaggerConfiguration:Title"] ?? configuration["ServiceName"],
                MajorVersion = Convert.ToInt32(configuration["SwaggerConfiguration:MajorVersion"]),
                MinorVersion = Convert.ToInt32(configuration["SwaggerConfiguration:MinorVersion"]),
                Description = configuration["SwaggerConfiguration:Description"]
            };

        internal class RestConfigurationBuilderContainer : RestConfiguration { }
    }
}
