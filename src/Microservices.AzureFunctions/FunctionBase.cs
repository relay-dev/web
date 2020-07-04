using Microsoft.Extensions.Logging;
using System;

namespace Microservices.AzureFunctions
{
    public class FunctionBase
    {
        protected TResult Execute<TResult>(Func<TResult> valueFactory, ILogger logger)
        {
            try
            {
                return valueFactory.Invoke();
            }
            catch (OperationCanceledException)
            {
                logger.LogInformation("Request was cancelled");

                throw;
            }
            catch (Exception e)
            {
                logger.LogError(e, "{0} failed with message: {1}", this.GetType(), e.Message);

                throw;
            }
        }
    }
}
