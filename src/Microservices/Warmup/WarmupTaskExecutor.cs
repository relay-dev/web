using Microservices.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Microservices.Warmup
{
    public class WarmupTaskExecutor
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly MicroserviceConfiguration _configuration;

        public WarmupTaskExecutor(IServiceProvider serviceProvider, MicroserviceConfiguration configuration)
        {
            _serviceProvider = serviceProvider;
            _configuration = configuration;
        }

        public async Task RunAsync(CancellationToken cancellationToken)
        {
            var tasks = new List<Task>();

            foreach (Type warmupType in _configuration.WarmupTypes)
            {
                var warmupTask = (WarmupTask)_serviceProvider.GetRequiredService(warmupType);

                tasks.Add(warmupTask.RunAsync(cancellationToken));
            }

            await Task.WhenAll(tasks);
        }
    }
}
