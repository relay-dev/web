using AutoMapper;
using Core.Framework;
using Core.Plugins.Configuration;
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
        private readonly PluginConfigurationBuilder _pluginConfigurationBuilder;

        public WebConfigurationBuilder(PluginConfigurationBuilder pluginConfigurationBuilder)
        {
            _webConfiguration = new WebConfiguration();
            _container = new WebConfigurationBuilderContainer();
            _pluginConfigurationBuilder = pluginConfigurationBuilder;
        }

        public WebConfigurationBuilder UseCommandHandlers(List<Type> commandHandlerTypes)
        {
            _webConfiguration.CommandHandlerTypes = commandHandlerTypes;

            return this;
        }

        public WebConfigurationBuilder UseCommandHandlersFromAssemblyContaining<TCommandHandler>()
        {
            _container.CommandHandlerAssemblies.Add(typeof(TCommandHandler).Assembly);

            return this;
        }

        public WebConfigurationBuilder UseCommandHandlersFromAssemblyContaining(Type type)
        {
            _container.CommandHandlerAssemblies.Add(type.Assembly);

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

        public WebConfigurationBuilder UseMappersFromAssemblyContaining(Type type)
        {
            _container.MapperAssemblies.Add(type.Assembly);

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

        public WebConfigurationBuilder UseValidatorsFromAssemblyContaining(Type type)
        {
            _webConfiguration.ValidatorsAssembly = type.Assembly;

            return this;
        }

        public WebConfiguration Build()
        {
            _webConfiguration.PluginConfiguration = _pluginConfigurationBuilder.Build();

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
