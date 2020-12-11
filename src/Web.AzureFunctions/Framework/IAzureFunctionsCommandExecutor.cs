using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;
using ExecutionContext = Microsoft.Azure.WebJobs.ExecutionContext;

namespace Web.AzureFunctions.Framework
{
    public interface IAzureFunctionsCommandExecutor
    {
        AzureFunctionsCommandExecutor Use(CancellationToken cancellationToken);
        AzureFunctionsCommandExecutor Use(ExecutionContext executionContext);
        AzureFunctionsCommandExecutor Use(ExecutionContext executionContext, ILogger logger, CancellationToken cancellationToken);
        AzureFunctionsCommandExecutor Use(ILogger logger);
        void Execute(Action valueFactory, ILogger logger);
        TResult Execute<TResult>(Func<TResult> valueFactory, ILogger logger);
        Task ExecuteAsync(Func<CancellationToken, Task> valueFactory);
        Task<TResult> ExecuteAsync<TResult>(Func<CancellationToken, Task<TResult>> valueFactory);
    }
}