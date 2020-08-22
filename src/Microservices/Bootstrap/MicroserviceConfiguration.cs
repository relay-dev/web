using Core.Application;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;

namespace Microservices.Bootstrap
{
    public class MicroserviceConfiguration
    {
        public string ServiceName { get; set; }
        public IConfiguration Configuration { get; set; }
        public SwaggerConfiguration SwaggerConfiguration { get; set; }
        public ApplicationContext ApplicationContext { get; set; }
        public List<Type> MapperTypes { get; set; }
        public List<Type> CommandHandlerTypes { get; set; }
        public Dictionary<Type, Type> ValidatorTypes { get; set; }
    }

    public class SwaggerConfiguration
    {
        public string Title { get; set; }
        public int MajorVersion { get; set; }
        public int MinorVersion { get; set; }
        public string Description { get; set; }
        public string Version => $"{MajorVersion}.{MinorVersion}";
    }
}
