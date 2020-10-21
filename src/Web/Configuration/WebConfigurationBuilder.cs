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
    public class WebConfigurationBuilder
    {
        private readonly WebConfiguration _webConfiguration;

        public WebConfigurationBuilder()
        {
            _webConfiguration = new WebConfiguration();
        }

        public WebConfigurationBuilder UseApplicationConfiguration(IConfiguration configuration)
        {
            _webConfiguration.ApplicationConfiguration = configuration;

            return this;
        }

        public WebConfigurationBuilder UseApplicationContext(ApplicationContext applicationContext)
        {
            _webConfiguration.ApplicationContext = applicationContext;

            return this;
        }

        public WebConfigurationBuilder UseApplicationName(string applicationName)
        {
            _webConfiguration.ApplicationName = applicationName;

            return this;
        }

        public WebConfigurationBuilder UseCommandHandlerTypes(List<Type> commandHandlerTypes)
        {
            _webConfiguration.CommandHandlerTypes = commandHandlerTypes;

            return this;
        }

        public WebConfigurationBuilder UseMapperTypes(List<Type> mapperTypes)
        {
            _webConfiguration.MapperTypes = mapperTypes;

            return this;
        }

        public WebConfigurationBuilder UseValidatorTypes(Dictionary<Type, Type> validatorTypes)
        {
            _webConfiguration.ValidatorTypes = validatorTypes;

            return this;
        }

        public WebConfigurationBuilder UseValidatorTypesFromAssembly(Assembly validatorsAssembly)
        {
            _webConfiguration.ValidatorsAssembly = validatorsAssembly;

            return this;
        }

        public WebConfigurationBuilder UseWarmupTypes(List<Type> warmupTypes)
        {
            _webConfiguration.WarmupTypes = warmupTypes;

            return this;
        }

        /// <notes>
        /// Limiting this to 1 assembly. Assembly scanning can become expensive in a cloud-based environment where applications need to auto-scale fast
        /// </notes>
        public WebConfigurationBuilder UseAssemblyToScan(Assembly assembly)
        {
            foreach (Type type in assembly.GetTypes())
            {
                if (type.GetInterfaces().Any(i => i.Name.Contains("IRequestHandler")))
                {
                    _webConfiguration.CommandHandlerTypes.Add(type);
                }
                else if (type.IsSubclassOf(typeof(Profile)))
                {
                    _webConfiguration.MapperTypes.Add(type);
                }
                else if (type.GetInterfaces().Contains(typeof(IWarmup)))
                {
                    _webConfiguration.WarmupTypes.Add(type);
                }
            }

            return this;
        }

        public WebConfiguration Build()
        {
            if (_webConfiguration.ApplicationConfiguration == null)
            {
                throw new InvalidOperationException("UseApplicationConfiguration() must be called before calling Build()");
            }

            _webConfiguration.ApplicationName ??= _webConfiguration.ApplicationConfiguration["ApplicationName"];

            if (string.IsNullOrEmpty(_webConfiguration.ApplicationName))
            {
                throw new InvalidOperationException("ApplicationName not provided. You can create an appSetting called 'ApplicationName', or call UseApplicationName() before calling Build()");
            }

            _webConfiguration.ApplicationContext ??= new ApplicationContext(_webConfiguration.ApplicationName);

            return _webConfiguration;
        }
    }
}
