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

        public WarmupTaskExecutor(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public async Task RunAsync(CancellationToken cancellationToken)
        {
            var tasks = new List<Task>();

            foreach (Type warmupType in WarmupTaskCollection.All)
            {
                var warmupTask = (WarmupTask)_serviceProvider.GetRequiredService(warmupType);

                tasks.Add(warmupTask.RunAsync(cancellationToken));
            }

            await Task.WhenAll(tasks);
        }
    }
}
