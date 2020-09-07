using Core.Application;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;

namespace Microservices.Configuration
{
    public class MicroserviceConfiguration
    {
        public MicroserviceConfiguration()
        {
            CommandHandlerTypes = new List<Type>();
            MapperTypes = new List<Type>();
            ValidatorTypes = new Dictionary<Type, Type>();
            WarmupTypes = new List<Type>();
        }

        public string ServiceName { get; set; }
        public IConfiguration Configuration { get; set; }
        public SwaggerConfiguration SwaggerConfiguration { get; set; }
        public ApplicationContext ApplicationContext { get; set; }
        public List<Type> CommandHandlerTypes { get; set; }
        public List<Type> MapperTypes { get; set; }
        public Dictionary<Type, Type> ValidatorTypes { get; set; }
        public List<Type> WarmupTypes { get; set; }
    }
}
