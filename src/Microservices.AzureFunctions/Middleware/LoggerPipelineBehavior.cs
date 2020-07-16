using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Microservices.AzureFunctions.Middleware
{
    public class LoggerPipelineBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    {
        private readonly ILogger<LoggerPipelineBehavior<TRequest, TResponse>> _logger;

        public LoggerPipelineBehavior(ILogger<LoggerPipelineBehavior<TRequest, TResponse>> logger)
        {
            _logger = logger;
        }

        public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
        {
            try
            {
                return await next();
            }
            catch (OperationCanceledException)
            {
                _logger.LogInformation("Request was cancelled");

                throw;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Operation failed");

                throw;
            }
        }
    }
}
