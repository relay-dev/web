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

        public WebConfigurationBuilder UseCommandHandlers(List<Type> commandHandlerTypes)
        {
            _webConfiguration.CommandHandlerTypes = commandHandlerTypes;

            return this;
        }

        public WebConfigurationBuilder UseCommandHandlersFromAssembly<TCommandHandler>()
        {
            _container.CommandHandlerAssemblies.Add(typeof(TCommandHandler).Assembly);
            
            return this;
        }

        public WebConfigurationBuilder UseMappers(List<Type> mapperTypes)
        {
            _webConfiguration.MapperTypes = mapperTypes;

            return this;
        }
        
        public WebConfigurationBuilder UseMappersFromAssemblyContaining<TMapper>()
        {
            _container.MapperAssemblies.Add(typeof(TMapper).Assembly);

            return this;
        }

        public WebConfigurationBuilder UseValidators(Dictionary<Type, Type> validatorTypes)
        {
            _webConfiguration.ValidatorTypes = validatorTypes;

            return this;
        }

        public WebConfigurationBuilder UseValidatorsFromAssemblyContaining<TValidator>()
        {
            _webConfiguration.ValidatorsAssembly = typeof(TValidator).Assembly;

            return this;
        }

        public WebConfigurationBuilder UseWarmups(List<Type> warmupTypes)
        {
            _webConfiguration.WarmupTypes = warmupTypes;

            return this;
        }

        public WebConfigurationBuilder UseWarmupsFromAssembly<TWarmup>()
        {
            _container.WarmupAssemblies.Add(typeof(TWarmup).Assembly);

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

            if (_container.CommandHandlerAssemblies.Any())
            {
                foreach (Type type in _container.CommandHandlerAssemblies.SelectMany(a => a.GetTypes()))
                {
                    if (type.GetInterfaces().Any(i => i.Name.Contains("IRequestHandler")))
                    {
                        _webConfiguration.CommandHandlerTypes.Add(type);
                    }
                }
            }

            if (_container.MapperAssemblies.Any())
            {
                foreach (Type type in _container.MapperAssemblies.SelectMany(a => a.GetTypes()))
                {
                    if (type.IsSubclassOf(typeof(Profile)))
                    {
                        _webConfiguration.MapperTypes.Add(type);
                    }
                }
            }

            if (_container.WarmupAssemblies.Any())
            {
                foreach (Type type in _container.WarmupAssemblies.SelectMany(a => a.GetTypes()))
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
                CommandHandlerAssemblies = new List<Assembly>();
                MapperAssemblies = new List<Assembly>();
                WarmupAssemblies = new List<Assembly>();
            }

            public List<Assembly> CommandHandlerAssemblies { get; set; }
            public List<Assembly> MapperAssemblies { get; set; }
            public List<Assembly> WarmupAssemblies { get; set; }
        }
    }
}
