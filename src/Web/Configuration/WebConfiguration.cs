using AutoMapper;
using Core.Application;
using Core.Framework;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Web.Configuration
{
    public class WebConfiguration
    {
        public WebConfiguration()
        {
            CommandHandlerTypes = new List<Type>();
            MapperTypes = new List<Type>();
            WarmupTypes = new List<Type>();
            ValidatorTypes = new Dictionary<Type, Type>();
        }

        public string ApplicationName { get; set; }
        public bool IsAddApiExplorer { get; set; }
        public IConfiguration Configuration { get; set; }
        public ApplicationContext ApplicationContext { get; set; }
        public List<Type> CommandHandlerTypes { get; set; }
        public List<Type> MapperTypes { get; set; }
        public List<Type> WarmupTypes { get; set; }
        public Dictionary<Type, Type> ValidatorTypes { get; set; }
        public Assembly ValidatorsAssembly { get; set; }

        /// <notes>
        /// Limiting this to 1 assembly. Assembly scanning can become expensive in a cloud-based environment where applications need to auto-scale fast
        /// </notes>
        public static WebConfiguration FromAssembly(Assembly assembly)
        {
            var config = new WebConfiguration();

            foreach (Type type in assembly.GetTypes())
            {
                if (type.GetInterfaces().Any(i => i.Name.Contains("IRequestHandler")))
                {
                    config.CommandHandlerTypes.Add(type);
                }
                else if (type.IsSubclassOf(typeof(Profile)))
                {
                    config.MapperTypes.Add(type);
                }
                else if (type.GetInterfaces().Contains(typeof(IWarmup)))
                {
                    config.WarmupTypes.Add(type);
                }
            }

            return config;
        }
    }
}
