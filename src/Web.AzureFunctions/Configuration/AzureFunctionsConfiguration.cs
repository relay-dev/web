using System;
using System.Collections.Generic;
using Web.Configuration;

namespace Web.AzureFunctions.Configuration
{
    public class AzureFunctionsConfiguration : WebConfiguration
    {
        public AzureFunctionsConfiguration() : base(false, true)
        {
            FunctionTypes = new List<Type>();
        }

        public bool IsEventHandler { get; set; }
        public List<Type> FunctionTypes { get; set; }
    }
}
