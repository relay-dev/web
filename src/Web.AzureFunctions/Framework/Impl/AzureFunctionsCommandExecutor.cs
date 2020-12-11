using Core.Plugins.Framework;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;
using ExecutionContext = Microsoft.Azure.WebJobs.ExecutionContext;

namespace Web.AzureFunctions.Framework
{
    public class AzureFunctionsCommandExecutor : IAzureFunctionsCommandExecutor
    {
        private readonly AzureFunctionsCommand _command;

        public AzureFunctionsCommandExecutor()
        {
            _command = new AzureFunctionsCommand();
        }

        public AzureFunctionsCommandExecutor Use(ExecutionContext executionContext)
        {
            _command.ExecutionContext = executionContext;

            return this;
        }

        public AzureFunctionsCommandExecutor Use(ILogger logger)
        {
            _command.Logger = logger;

            return this;
        }

        public AzureFunctionsCommandExecutor Use(CancellationToken cancellationToken)
        {
            _command.CancellationToken = cancellationToken;

            return this;
        }

        public AzureFunctionsCommandExecutor Use(ExecutionContext executionContext, ILogger logger, CancellationToken cancellationToken)
        {
            _command.ExecutionContext = executionContext;
            _command.Logger = logger;
            _command.CancellationToken = cancellationToken;

            return this;
        }

        public async Task<TResult> ExecuteAsync<TResult>(Func<CancellationToken, Task<TResult>> valueFactory)
        {
            Validate();

            try
            {
                return await valueFactory.Invoke(_command.CancellationToken);
            }
            catch (Exception e) when (!(e is OperationCanceledException))
            {
                _command.Logger.LogError(e, "{0} failed with message: {1}", _command.ExecutionContext.FunctionName, e.Message);

                throw;
            }
        }

        public async Task ExecuteAsync(Func<CancellationToken, Task> valueFactory)
        {
            Validate();

            try
            {
                await valueFactory.Invoke(_command.CancellationToken);
            }
            catch (Exception e) when (!(e is OperationCanceledException))
            {
                _command.Logger.LogError(e, "{0} failed with message: {1}", _command.ExecutionContext.FunctionName, e.Message);

                throw;
            }
        }

        public TResult Execute<TResult>(Func<TResult> valueFactory, ILogger logger)
        {
            Validate();

            try
            {
                return valueFactory.Invoke();
            }
            catch (Exception e) when (!(e is OperationCanceledException))
            {
                logger.LogError(e, "{0} failed with message: {1}", _command.ExecutionContext.FunctionName, e.Message);

                throw;
            }
        }

        public void Execute(Action valueFactory, ILogger logger)
        {
            Validate();

            try
            {
                valueFactory.Invoke();
            }
            catch (Exception e) when (!(e is OperationCanceledException))
            {
                logger.LogError(e, "{0} failed with message: {1}", _command.ExecutionContext.FunctionName, e.Message);

                throw;
            }
        }

        private void Validate()
        {
            _command.ExecutionContext.ThrowIfNull();
            _command.Logger.ThrowIfNull();
        }

        internal class AzureFunctionsCommand
        {
            public ExecutionContext ExecutionContext { get; set; }
            public ILogger Logger { get; set; }
            public CancellationToken CancellationToken { get; set; }
        }
    }
}
