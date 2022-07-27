using System;
using System.Collections.Generic;
using System.Reflection;
using Web.Configuration;

namespace Web.AzureFunctions.Configuration
{
    public class AzureFunctionsConfiguration : WebConfiguration
    {
        public AzureFunctionsConfiguration() : base(false, true)
        {
            FunctionTypes = new List<Type>();
            FunctionAssemblies = new List<Assembly>();
        }

        public bool IsEventHandler { get; set; }
        public List<Type> FunctionTypes { get; set; }
        public List<Assembly> FunctionAssemblies { get; set; }
    }
}
