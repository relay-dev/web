using Core.Plugins.Configuration;
using System;

namespace Web.Configuration
{
    public class WebConfigurationBuilder : WebConfigurationBuilder<WebConfigurationBuilder, WebConfiguration>
    {

    }

    public class WebConfigurationBuilder<TBuilder, TResult> : PluginConfigurationBuilder<TBuilder, TResult> where TBuilder : class where TResult : class
    {
        private readonly WebConfigurationBuilderContainer _container;

        public WebConfigurationBuilder()
        {
            _container = new WebConfigurationBuilderContainer();
        }

        public TBuilder UseDiagnostics(bool flag = true)
        {
            _container.IsAddDiagnostics = flag;

            return this as TBuilder;
        }

        public override TResult Build()
        {
            var webConfiguration = new WebConfiguration();

            return BuildUsing(webConfiguration);
        }

        protected override TResult BuildUsing<TConfiguration>(TConfiguration configuration)
        {
            var webConfiguration = configuration as WebConfiguration;

            if (webConfiguration == null)
            {
                throw new InvalidOperationException("webConfiguration cannot be null");
            }

            base.BuildUsing(webConfiguration);

            webConfiguration.IsAddDiagnostics = _container.IsAddDiagnostics;

            return webConfiguration as TResult;
        }
        
        internal class WebConfigurationBuilderContainer : WebConfiguration
        {
            
        }
    }
}
