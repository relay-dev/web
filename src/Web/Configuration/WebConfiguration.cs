using Core.Plugins.Configuration;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Web.Configuration
{
    public class WebConfiguration : PluginConfiguration
    {
        public WebConfiguration()
        {
            IsAddDiagnostics = true;
            CommandHandlerTypes = new List<Type>();
            MapperTypes = new List<Type>();
            ValidatorTypes = new Dictionary<Type, Type>();
            ValidatorsAssemblies = new List<Assembly>();
        }

        public bool IsAddApiExplorer { get; set; }
        public bool IsAddDiagnostics { get; set; }
        public List<Type> CommandHandlerTypes { get; set; }
        public List<Type> MapperTypes { get; set; }
        public Dictionary<Type, Type> ValidatorTypes { get; set; }
        public List<Assembly> ValidatorsAssemblies { get; set; }
    }
}
