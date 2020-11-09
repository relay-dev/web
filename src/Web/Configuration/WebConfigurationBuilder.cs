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
        private readonly WebConfigurationBuilderContainer _container;

        public WebConfigurationBuilder()
        {
            _webConfiguration = new WebConfiguration();
            _container = new WebConfigurationBuilderContainer();
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

        public WebConfigurationBuilder UseCommandHandlerTypesFromAssembly<TCommandHandler>()
        {
            _container.CommandHandlerTypesAssemblies.Add(typeof(TCommandHandler).Assembly);
            
            return this;
        }

        public WebConfigurationBuilder UseMapperTypes(List<Type> mapperTypes)
        {
            _webConfiguration.MapperTypes = mapperTypes;

            return this;
        }
        
        public WebConfigurationBuilder UseMapperTypesFromAssemblyContaining<TMapper>()
        {
            _container.MapperTypesAssemblies.Add(typeof(TMapper).Assembly);

            return this;
        }

        public WebConfigurationBuilder UseValidatorTypes(Dictionary<Type, Type> validatorTypes)
        {
            _webConfiguration.ValidatorTypes = validatorTypes;

            return this;
        }

        public WebConfigurationBuilder UseValidatorTypesFromAssemblyContaining<TValidator>()
        {
            _webConfiguration.ValidatorsAssembly = typeof(TValidator).Assembly;

            return this;
        }

        public WebConfigurationBuilder UseWarmupTypes(List<Type> warmupTypes)
        {
            _webConfiguration.WarmupTypes = warmupTypes;

            return this;
        }

        public WebConfigurationBuilder UseWarmupTypesFromAssembly<TWarmup>()
        {
            _container.WarmupTypesAssemblies.Add(typeof(TWarmup).Assembly);

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

            if (_container.CommandHandlerTypesAssemblies.Any())
            {
                foreach (Type type in _container.CommandHandlerTypesAssemblies.SelectMany(a => a.GetTypes()))
                {
                    if (type.GetInterfaces().Any(i => i.Name.Contains("IRequestHandler")))
                    {
                        _webConfiguration.CommandHandlerTypes.Add(type);
                    }
                }
            }

            if (_container.MapperTypesAssemblies.Any())
            {
                foreach (Type type in _container.MapperTypesAssemblies.SelectMany(a => a.GetTypes()))
                {
                    if (type.IsSubclassOf(typeof(Profile)))
                    {
                        _webConfiguration.MapperTypes.Add(type);
                    }
                }
            }

            if (_container.WarmupTypesAssemblies.Any())
            {
                foreach (Type type in _container.WarmupTypesAssemblies.SelectMany(a => a.GetTypes()))
                {
                    if (type.GetInterfaces().Contains(typeof(IWarmup)))
                    {
                        _webConfiguration.WarmupTypes.Add(type);
                    }
                }
            }

            return _webConfiguration;
        }

        internal class WebConfigurationBuilderContainer
        {
            public WebConfigurationBuilderContainer()
            {
                CommandHandlerTypesAssemblies = new List<Assembly>();
                MapperTypesAssemblies = new List<Assembly>();
                WarmupTypesAssemblies = new List<Assembly>();
            }

            public List<Assembly> CommandHandlerTypesAssemblies { get; set; }
            public List<Assembly> MapperTypesAssemblies { get; set; }
            public List<Assembly> WarmupTypesAssemblies { get; set; }
        }
    }
}
