using Microsoft.Extensions.Logging;
using System;

namespace Microservices.AzureFunctions
{
    public class FunctionBase
    {
        protected readonly ILogger Logger;

        public FunctionBase(ILogger logger)
        {
            Logger = logger;
        }

        protected TResult Execute<TResult>(Func<TResult> valueFactory)
        {
            TResult result = default;

            try
            {
                result = valueFactory.Invoke();
            }
            catch (OperationCanceledException)
            {
                Logger.LogInformation("Request was cancelled");
            }
            catch (Exception e)
            {
                Logger.LogError(e, "{0} failed with message: {1}", this.GetType(), e.Message);
            }

            return result;
        }
    }
}
