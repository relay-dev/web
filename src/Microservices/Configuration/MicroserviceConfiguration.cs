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
            MapperTypes = new List<Type>();
            CommandHandlerTypes = new List<Type>();
            ValidatorTypes = new Dictionary<Type, Type>();
        }

        public string ServiceName { get; set; }
        public IConfiguration Configuration { get; set; }
        public SwaggerConfiguration SwaggerConfiguration { get; set; }
        public ApplicationContext ApplicationContext { get; set; }
        public List<Type> MapperTypes { get; set; }
        public List<Type> CommandHandlerTypes { get; set; }
        public Dictionary<Type, Type> ValidatorTypes { get; set; }
    }
}
