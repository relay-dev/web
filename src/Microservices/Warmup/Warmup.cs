using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Microservices.Warmup
{
    public class Warmup
    {
        private readonly IServiceProvider _serviceProvider;

        public Warmup(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public async Task Run()
        {
            var tasks = new List<Task>();

            foreach (Type warmupType in WarmupTasks.All)
            {
                WarmupTask warmupTask = _serviceProvider.GetRequiredService(warmupType) as WarmupTask;

                tasks.Add(Task.Run(() => warmupTask.Run()));
            }

            await Task.WhenAll(tasks);
        }
    }
}
