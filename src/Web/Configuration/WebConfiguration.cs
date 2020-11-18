using Core.Application;
using Core.Plugins.Configuration;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Web.Configuration
{
    public class WebConfiguration
    {
        public WebConfiguration()
        {
            CommandHandlerTypes = new List<Type>();
            MapperTypes = new List<Type>();
            ValidatorTypes = new Dictionary<Type, Type>();
        }

        public bool IsAddApiExplorer { get; set; }
        public List<Type> CommandHandlerTypes { get; set; }
        public List<Type> MapperTypes { get; set; }
        public Dictionary<Type, Type> ValidatorTypes { get; set; }
        public Assembly ValidatorsAssembly { get; set; }

        public PluginConfiguration PluginConfiguration { get; set; }

        public string ApplicationName => PluginConfiguration.ApplicationConfiguration.ApplicationName;
        public ApplicationContext ApplicationContext => PluginConfiguration.ApplicationConfiguration.ApplicationContext;
        public IConfiguration ApplicationConfiguration => PluginConfiguration.ApplicationConfiguration.Configuration;
        public List<Type> WarmupTypes => PluginConfiguration.ApplicationConfiguration.WarmupTypes;
    }
}
