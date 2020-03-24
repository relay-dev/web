using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace Microservices.Warmup
{
    public abstract class WarmupTask : IWarmup
    {
        private readonly ILogger _logger;

        public WarmupTask(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger("Default");
        }

        protected abstract void OnWarmup();

        public void Run()
        {
            var stopwatch = Stopwatch.StartNew();

            OnWarmup();

            stopwatch.Stop();

            _logger.LogInformation($"Warmup Task of type '{GetType().Name}' completed in {stopwatch.ElapsedMilliseconds}ms");
        }
    }
}
