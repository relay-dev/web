using Microsoft.Extensions.Logging;
using System;

namespace Microservices.Extensions
{
    public static class LoggerExtensions
    {
        public static void LogError(this ILogger logger, Exception e, string message, params object[] args)
        {
            logger.LogError(default, e, message, args);
        }
    }
}
