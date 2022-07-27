using Microsoft.Extensions.Configuration;
using System;
using System.Linq;
using Web.AzureFunctions.Configuration.Options;
using Web.AzureFunctions.Framework;
using Web.Configuration;

namespace Web.AzureFunctions.Configuration
{
    public class AzureFunctionsConfigurationBuilder : AzureFunctionsConfigurationBuilder<AzureFunctionsConfigurationBuilder, AzureFunctionsConfiguration>
    {
        public AzureFunctionsConfigurationBuilder() { }

        public AzureFunctionsConfigurationBuilder(IConfiguration configuration)
        {
            UseConfiguration(configuration);
        }
    }

    public class AzureFunctionsConfigurationBuilder<TBuilder, TResult> : WebConfigurationBuilder<TBuilder, TResult> where TBuilder : class where TResult : class
    {
        private readonly AzureFunctionsConfiguration _azureFunctionsConfiguration;

        public AzureFunctionsConfigurationBuilder()
        {
            _azureFunctionsConfiguration = new AzureFunctionsConfiguration();
        }

        public TBuilder UseFunctions(Action<AzureFunctionsOptions> options)
        {
            var azureFunctionsOptions = new AzureFunctionsOptions(_azureFunctionsConfiguration);

            options.Invoke(azureFunctionsOptions);

            return this as TBuilder;
        }

        public TBuilder AsEventHandler()
        {
            _azureFunctionsConfiguration.IsEventHandler = true;

            return this as TBuilder;
        }

        public override TResult Build()
        {
            var azureFunctionsConfiguration = new AzureFunctionsConfiguration();

            return BuildUsing(azureFunctionsConfiguration);
        }

        protected override TResult BuildUsing<TConfiguration>(TConfiguration configuration)
        {
            var azureFunctionsConfiguration = configuration as AzureFunctionsConfiguration;

            if (azureFunctionsConfiguration == null)
            {
                throw new InvalidOperationException("azureFunctionsConfiguration cannot be null");
            }

            base.BuildUsing(azureFunctionsConfiguration);

            if (_azureFunctionsConfiguration.FunctionTypes.Any())
            {
                azureFunctionsConfiguration.FunctionTypes = _azureFunctionsConfiguration.FunctionTypes;
            }
            
            if (_azureFunctionsConfiguration.FunctionAssemblies.Any())
            {
                foreach (Type type in _azureFunctionsConfiguration.FunctionAssemblies.SelectMany(a => a.GetTypes()))
                {
                    if (type.IsSubclassOf(typeof(AzureFunctionBase)) || type == typeof(AzureFunctionBase))
                    {
                        azureFunctionsConfiguration.FunctionTypes.Add(type);
                    }
                }
            }

            azureFunctionsConfiguration.IsEventHandler = _azureFunctionsConfiguration.IsEventHandler;

            return azureFunctionsConfiguration as TResult;
        }
    }
}
