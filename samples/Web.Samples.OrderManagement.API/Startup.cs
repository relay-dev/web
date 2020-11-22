using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Web.Rest;
using Web.Rest.Configuration;
using Web.Samples.OrderManagement.API.Extensions;
using Web.Samples.OrderManagement.Domain.Commands.Create;
using Web.Samples.OrderManagement.Domain.Context;
using Web.Samples.OrderManagement.Domain.Mappers;
using Web.Samples.OrderManagement.Domain.Validators;

namespace Web.Samples.OrderManagement.API
{
    public class Startup
    {
        private readonly RestConfiguration _restConfiguration;

        public Startup(IConfiguration configuration)
        {
            _restConfiguration = BuildRestConfiguration(configuration);
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddRestFramework<OrderContext>(_restConfiguration);

            services.AddSampleServices();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseRestFramework(_restConfiguration, env);
        }

        private RestConfiguration BuildRestConfiguration(IConfiguration configuration)
        {
            return new RestConfigurationBuilder()
                .UseConfiguration(configuration)
                .UseApplicationName(GetType().AssemblyQualifiedName)
                .UseCommandHandlersFromAssemblyContaining<CreateOrderHandler>()
                .UseMappersFromAssemblyContaining<AutoMappers>()
                .UseValidatorsFromAssemblyContaining<OrderValidator>()
                .Build();
        }
    }
}
