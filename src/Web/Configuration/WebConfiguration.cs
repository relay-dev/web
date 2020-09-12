using Core.Application;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;

namespace Web.Configuration
{
    public class WebConfiguration
    {
        public WebConfiguration()
        {
            CommandHandlerTypes = new List<Type>();
            MapperTypes = new List<Type>();
            WarmupTypes = new List<Type>();
            ValidatorTypes = new Dictionary<Type, Type>();
        }

        public string ApplicationName { get; set; }
        public IConfiguration Configuration { get; set; }
        public ApplicationContext ApplicationContext { get; set; }
        public List<Type> CommandHandlerTypes { get; set; }
        public List<Type> MapperTypes { get; set; }
        public List<Type> WarmupTypes { get; set; }
        public Dictionary<Type, Type> ValidatorTypes { get; set; }
    }
}
