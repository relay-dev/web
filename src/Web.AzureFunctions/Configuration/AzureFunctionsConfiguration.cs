using System;
using System.Collections.Generic;
using Web.Configuration;

namespace Web.AzureFunctions.Configuration
{
    public class AzureFunctionsConfiguration : WebConfiguration
    {
        public AzureFunctionsConfiguration()
        {
            FunctionTypes = new List<Type>();
        }

        public bool IsEventHandler { get; set; }
        public List<Type> FunctionTypes { get; set; }

        public bool IsLocal => bool.Parse(Environment.GetEnvironmentVariable("IS_LOCAL") ?? false.ToString());
    }
}
