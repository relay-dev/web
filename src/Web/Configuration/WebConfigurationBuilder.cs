using AutoMapper;
using Core.Framework;
using System;
using System.Linq;
using System.Reflection;

namespace Web.Configuration
{
    public class WebConfigurationBuilder
    {
        /// <notes>
        /// Limiting this to 1 assembly. Assembly scanning can become expensive in a cloud-based environment where applications need to auto-scale fast
        /// </notes>
        public WebConfiguration Build(Assembly assembly)
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
