using System.Reflection;
using Core.Application;
using Microsoft.Extensions.Configuration;

namespace Microservices.Bootstrap
{
    public class MicroserviceConfiguration
    {
        public string ServiceName { get; set; }
        public IConfiguration Configuration { get; set; }
        public Assembly[] AssembliesToScan { get; set; }
        public SwaggerConfiguration SwaggerConfiguration { get; set; }
        public ApplicationContext ApplicationContext { get; set; }
    }

    public class SwaggerConfiguration
    {
        public string Title { get; set; }
        public string Version { get; set; }
        public string Description { get; set; }
    }
}
