using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using Web.Configuration;

namespace Web.AzureFunctions.Configuration
{
    public class AzureFunctionsConfiguration
    {
        public AzureFunctionsConfiguration()
        {
            FunctionTypes = new List<Type>();
        }

        public bool IsEventHandler { get; set; }
        public List<Type> FunctionTypes { get; set; }
        public WebConfiguration WebConfiguration { get; set; }
        public IConfiguration ApplicationConfiguration => WebConfiguration.ApplicationConfiguration;

        public bool IsLocal => bool.Parse(Environment.GetEnvironmentVariable("IS_LOCAL") ?? false.ToString());
    }
}
