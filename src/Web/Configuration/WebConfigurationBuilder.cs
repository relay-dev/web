using AutoMapper;
using Core.Plugins.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Web.Configuration
{
    public class WebConfigurationBuilder : WebConfigurationBuilder<WebConfigurationBuilder, WebConfiguration>
    {

    }

    public class WebConfigurationBuilder<TBuilder, TResult> : PluginConfigurationBuilder<TBuilder, TResult> where TBuilder : class where TResult : class
    {
        private readonly WebConfigurationBuilderContainer _container;

        public WebConfigurationBuilder()
        {
            _container = new WebConfigurationBuilderContainer();
        }

        public TBuilder UseCommandHandlers(List<Type> commandHandlerTypes)
        {
            _container.CommandHandlerTypes = commandHandlerTypes;

            return this as TBuilder;
        }

        public TBuilder UseCommandHandlersFromAssemblyContaining<TCommandHandler>()
        {
            _container.CommandHandlerAssemblies.Add(typeof(TCommandHandler).Assembly);

            return this as TBuilder;
        }

        public TBuilder UseCommandHandlersFromAssemblyContaining(Type type)
        {
            _container.CommandHandlerAssemblies.Add(type.Assembly);

            return this as TBuilder;
        }

        public TBuilder UseMappers(List<Type> mapperTypes)
        {
            _container.MapperTypes = mapperTypes;

            return this as TBuilder;
        }

        public TBuilder UseMappersFromAssemblyContaining<TMapper>()
        {
            _container.MapperAssemblies.Add(typeof(TMapper).Assembly);

            return this as TBuilder;
        }

        public TBuilder UseMappersFromAssemblyContaining(Type type)
        {
            _container.MapperAssemblies.Add(type.Assembly);

            return this as TBuilder;
        }

        public TBuilder UseValidators(Dictionary<Type, Type> validatorTypes)
        {
            _container.ValidatorTypes = validatorTypes;

            return this as TBuilder;
        }

        public TBuilder UseValidatorsFromAssemblyContaining<TValidator>()
        {
            _container.ValidatorAssemblies.Add(typeof(TValidator).Assembly);

            return this as TBuilder;
        }

        public TBuilder UseValidatorsFromAssemblyContaining(Type type)
        {
            _container.ValidatorAssemblies.Add(type.Assembly);

            return this as TBuilder;
        }

        public TBuilder UseDiagnostics(bool flag = true)
        {
            _container.IsAddDiagnostics = flag;

            return this as TBuilder;
        }

        public TBuilder UseApiExplorer(bool flag = true)
        {
            _container.IsAddApiExplorer = flag;

            return this as TBuilder;
        }

        public override TResult Build()
        {
            var webConfiguration = new WebConfiguration();

            return BuildUsing(webConfiguration);
        }

        protected override TResult BuildUsing<TConfiguration>(TConfiguration configuration)
        {
            var webConfiguration = configuration as WebConfiguration;

            if (webConfiguration == null)
            {
                throw new InvalidOperationException("webConfiguration cannot be null");
            }

            base.BuildUsing(webConfiguration);

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

            webConfiguration.IsAddDiagnostics = _container.IsAddDiagnostics;
            webConfiguration.IsAddApiExplorer = _container.IsAddApiExplorer;

            return webConfiguration as TResult;
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
