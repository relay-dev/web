using System;
using System.Collections.Generic;

namespace Web.AzureFunctions.Configuration.Options
{
    public class AzureFunctionsOptions
    {
        private readonly AzureFunctionsConfiguration _azureFunctionsConfiguration;

        public AzureFunctionsOptions(AzureFunctionsConfiguration azureFunctionsConfiguration)
        {
            _azureFunctionsConfiguration = azureFunctionsConfiguration;
        }

        public AzureFunctionsOptions FromAssemblyContaining<TFunctions>()
        {
            _azureFunctionsConfiguration.FunctionAssemblies.Add(typeof(TFunctions).Assembly);

            return this;
        }

        public AzureFunctionsOptions FromAssemblyContaining(Type type)
        {
            _azureFunctionsConfiguration.FunctionAssemblies.Add(type.Assembly);

            return this;
        }

        public AzureFunctionsOptions FromCollection(List<Type> functionTypes)
        {
            _azureFunctionsConfiguration.FunctionTypes = functionTypes;

            return this;
        }
    }
}
