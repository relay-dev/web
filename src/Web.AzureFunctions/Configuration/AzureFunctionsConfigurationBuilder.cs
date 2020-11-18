using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Web.AzureFunctions.Framework;
using Web.Configuration;

namespace Web.AzureFunctions.Configuration
{
    public class AzureFunctionsConfigurationBuilder
    {
        private readonly AzureFunctionsConfiguration _azureFunctionsConfiguration;
        private readonly AzureFunctionConfigurationBuilderContainer _container;
        private readonly WebConfigurationBuilder _webConfigurationBuilder;

        public AzureFunctionsConfigurationBuilder(WebConfigurationBuilder webConfigurationBuilder)
        {
            _webConfigurationBuilder = webConfigurationBuilder;
            _azureFunctionsConfiguration = new AzureFunctionsConfiguration();
            _container = new AzureFunctionConfigurationBuilderContainer();
        }
        
        public AzureFunctionsConfigurationBuilder UseFunctions(List<Type> functionTypes)
        {
            _azureFunctionsConfiguration.FunctionTypes = functionTypes;

            return this;
        }

        public AzureFunctionsConfigurationBuilder UseFunctionsFromAssemblyContaining<TFunction>()
        {
            _container.FunctionsAssemblies.Add(typeof(TFunction).Assembly);

            return this;
        }

        public AzureFunctionsConfigurationBuilder UseFunctionsFromAssemblyContaining(Type type)
        {
            _container.FunctionsAssemblies.Add(type.Assembly);

            return this;
        }

        public AzureFunctionsConfigurationBuilder AsEventHandler()
        {
            _azureFunctionsConfiguration.IsEventHandler = true;

            return this;
        }

        public AzureFunctionsConfiguration Build()
        {
            _azureFunctionsConfiguration.WebConfiguration = _webConfigurationBuilder.Build();

            if (_container.FunctionsAssemblies.Any())
            {
                foreach (Type type in _container.FunctionsAssemblies.SelectMany(a => a.GetTypes()))
                {
                    if (type.IsSubclassOf(typeof(AzureFunctionBase)) || type == typeof(AzureFunctionBase))
                    {
                        _azureFunctionsConfiguration.FunctionTypes.Add(type);
                    }
                }
            }

            return _azureFunctionsConfiguration;
        }

        internal class AzureFunctionConfigurationBuilderContainer
        {
            public AzureFunctionConfigurationBuilderContainer()
            {
                FunctionsAssemblies = new List<Assembly>();
            }

            public List<Assembly> FunctionsAssemblies { get; set; }
        }
    }
}
