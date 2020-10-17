using Microsoft.Extensions.Configuration;
using System;
using Web.Configuration;

namespace Web.AzureFunctions.Configuration
{
    public class AzureFunctionsConfiguration
    {
        public AzureFunctionsConfiguration() { }

        public AzureFunctionsConfiguration(WebConfiguration webConfiguration)
        {
            WebConfiguration = webConfiguration;
        }

        public bool IsEventHandler { get; set; }
        public WebConfiguration WebConfiguration { get; set; }
        public IConfiguration ApplicationConfiguration => WebConfiguration.ApplicationConfiguration;

        public bool IsLocal => bool.Parse(Environment.GetEnvironmentVariable("IS_LOCAL") ?? false.ToString());
    }
}
