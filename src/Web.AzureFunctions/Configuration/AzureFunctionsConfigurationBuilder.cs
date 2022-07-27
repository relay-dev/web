using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
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
        private readonly AzureFunctionsConfiguration _container;

        public AzureFunctionsConfigurationBuilder()
        {
            _container = new AzureFunctionsConfiguration();
        }
        
        public TBuilder UseFunctions(List<Type> functionTypes)
        {
            _container.FunctionTypes = functionTypes;

            return this as TBuilder;
        }

        public TBuilder UseFunctionsFromAssemblyContaining<TFunction>()
        {
            _container.FunctionAssemblies.Add(typeof(TFunction).Assembly);

            return this as TBuilder;
        }

        public TBuilder UseFunctionsFromAssemblyContaining(Type type)
        {
            _container.FunctionAssemblies.Add(type.Assembly);

            return this as TBuilder;
        }

        public TBuilder AsEventHandler()
        {
            _container.IsEventHandler = true;

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

            if (_container.FunctionTypes.Any())
            {
                azureFunctionsConfiguration.FunctionTypes = _container.FunctionTypes;
            }
            
            if (_container.FunctionAssemblies.Any())
            {
                foreach (Type type in _container.FunctionAssemblies.SelectMany(a => a.GetTypes()))
                {
                    if (type.IsSubclassOf(typeof(AzureFunctionBase)) || type == typeof(AzureFunctionBase))
                    {
                        azureFunctionsConfiguration.FunctionTypes.Add(type);
                    }
                }
            }

            azureFunctionsConfiguration.IsEventHandler = _container.IsEventHandler;

            return azureFunctionsConfiguration as TResult;
        }
    }
}
