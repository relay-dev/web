using Core.Events;
using Microsoft.Extensions.DependencyInjection;
using Web.Samples.OrderManagement.Domain.Events.Client;

namespace Web.Samples.OrderManagement.API.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddSampleServices(this IServiceCollection services)
        {
            services.AddTransient<IEventClient, MockEventClient>();

            return services;
        }
    }
}
