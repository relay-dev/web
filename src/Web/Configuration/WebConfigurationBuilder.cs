using AutoMapper;
using Core.Plugins.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Web.Configuration
{
    public class WebConfigurationBuilder<TConfiguration> : PluginConfigurationBuilder<TConfiguration> where TConfiguration : class
    {
        private readonly WebConfigurationBuilderContainer _container;

        public WebConfigurationBuilder()
        {
            _container = new WebConfigurationBuilderContainer();
        }

        public WebConfigurationBuilder<TConfiguration> UseCommandHandlers(List<Type> commandHandlerTypes)
        {
            _container.CommandHandlerTypes = commandHandlerTypes;

            return this;
        }

        public WebConfigurationBuilder<TConfiguration> UseCommandHandlersFromAssemblyContaining<TCommandHandler>()
        {
            _container.CommandHandlerAssemblies.Add(typeof(TCommandHandler).Assembly);

            return this;
        }

        public WebConfigurationBuilder<TConfiguration> UseCommandHandlersFromAssemblyContaining(Type type)
        {
            _container.CommandHandlerAssemblies.Add(type.Assembly);

            return this;
        }

        public WebConfigurationBuilder<TConfiguration> UseMappers(List<Type> mapperTypes)
        {
            _container.MapperTypes = mapperTypes;

            return this;
        }

        public WebConfigurationBuilder<TConfiguration> UseMappersFromAssemblyContaining<TMapper>()
        {
            _container.MapperAssemblies.Add(typeof(TMapper).Assembly);

            return this;
        }

        public WebConfigurationBuilder<TConfiguration> UseMappersFromAssemblyContaining(Type type)
        {
            _container.MapperAssemblies.Add(type.Assembly);

            return this;
        }

        public WebConfigurationBuilder<TConfiguration> UseValidators(Dictionary<Type, Type> validatorTypes)
        {
            _container.ValidatorTypes = validatorTypes;

            return this;
        }

        public WebConfigurationBuilder<TConfiguration> UseValidatorsFromAssemblyContaining<TValidator>()
        {
            _container.ValidatorAssemblies.Add(typeof(TValidator).Assembly);

            return this;
        }

        public WebConfigurationBuilder<TConfiguration> UseValidatorsFromAssemblyContaining(Type type)
        {
            _container.ValidatorAssemblies.Add(type.Assembly);

            return this;
        }

        public override TConfiguration Build()
        {
            var webConfiguration = base.Build() as WebConfiguration;

            if (webConfiguration == null)
            {
                throw new InvalidOperationException("webConfiguration cannot be null");
            }

            if (_container.CommandHandlerTypes.Any())
            {
                webConfiguration.CommandHandlerTypes = _container.CommandHandlerTypes;
            }

            if (_container.CommandHandlerAssemblies.Any())
            {
                foreach (Type type in _container.CommandHandlerAssemblies.SelectMany(a => a.GetTypes()))
                {
                    if (type.GetInterfaces().Any(i => i.Name.Contains("IRequestHandler")))
                    {
                        webConfiguration.CommandHandlerTypes.Add(type);
                    }
                }
            }

            if (_container.MapperTypes.Any())
            {
                webConfiguration.MapperTypes = _container.MapperTypes;
            }

            if (_container.MapperAssemblies.Any())
            {
                foreach (Type type in _container.MapperAssemblies.SelectMany(a => a.GetTypes()))
                {
                    if (type.IsSubclassOf(typeof(Profile)))
                    {
                        webConfiguration.MapperTypes.Add(type);
                    }
                }
            }

            if (_container.ValidatorTypes.Any())
            {
                webConfiguration.ValidatorTypes = _container.ValidatorTypes;
            }

            if (_container.ValidatorAssemblies.Any())
            {
                webConfiguration.ValidatorsAssemblies.AddRange(_container.ValidatorAssemblies);
            }

            return webConfiguration as TConfiguration;
        }

        internal class WebConfigurationBuilderContainer : WebConfiguration
        {
            public WebConfigurationBuilderContainer()
            {
                CommandHandlerAssemblies = new List<Assembly>();
                MapperAssemblies = new List<Assembly>();
                ValidatorAssemblies = new List<Assembly>();
            }

            public List<Assembly> CommandHandlerAssemblies { get; set; }
            public List<Assembly> MapperAssemblies { get; set; }
            public List<Assembly> ValidatorAssemblies { get; set; }
        }
    }
}
