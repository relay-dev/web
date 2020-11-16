using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Web.AzureFunctions.Framework
{
    public class FunctionBase
    {
        protected async Task<TResult> ExecuteAsync<TResult>(Func<Task<TResult>> valueFactory, ILogger logger, CancellationToken cancellationToken)
        {
            try
            {
                return await valueFactory.Invoke();
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

//    public abstract class FunctionStartupBase : FunctionsStartup
//    {
//        public override void ConfigureAppConfiguration(IFunctionsConfigurationBuilder builder)
//        {
//            FunctionsHostBuilderContext context = builder.GetContext();

//            string basePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory.SubstringBefore("tests"), "src", typeof(TStartup).Namespace);

//            configBuilder
//                .SetBasePath(basePath)
//                .AddJsonFile("appsettings.json", true, true)
//                .AddJsonFile("appsettings.Development.json", true, true)
//                .AddJsonFile("appsettings.Local.json", true, true)
//                .AddUserSecrets<TStartup>()
//                .AddEnvironmentVariables();

//            builder.ConfigurationBuilder
//                .AddJsonFile(Path.Combine(context.ApplicationRootPath, "appsettings.json"), optional: true, reloadOnChange: false)
//                .AddJsonFile(Path.Combine(context.ApplicationRootPath, $"appsettings.{context.EnvironmentName}.json"), optional: true, reloadOnChange: false)
//                .AddEnvironmentVariables();

//            base.ConfigureAppConfiguration(builder);
//        }
//    }
}
