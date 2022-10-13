using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using Web.Testing.Integration;
using ExecutionContext = Microsoft.Azure.WebJobs.ExecutionContext;

namespace Web.Testing.AzureFunctions.Integration
{
    public abstract class FunctionIntegrationTest : WebIntegrationTest
    {
        /// <summary>
        /// The execution context from the current thread
        /// </summary>
        protected virtual ExecutionContext ExecutionContext => new ExecutionContext();
    }

    public abstract class FunctionIntegrationTest<TSUT> : FunctionIntegrationTest
    {
        public override void BootstrapTest()
        {
            base.BootstrapTest();

            // Get this test's service provider. It was set on the test's current context by the BootstrapTest() method.
            var serviceProvider = (IServiceProvider)CurrentTestProperties.Get(ServiceProviderKey);

            // Use this test's service provider to resolve the service of the type we are testing
            TSUT sut = serviceProvider.GetRequiredService<TSUT>();

            // Set the instance on this test's context so we can reference it in SUT
            CurrentTestProperties.Set(SutKey, sut);
        }

        /// <summary>
        /// Returns a new SUT everytime it is called
        /// </summary>
        protected virtual TSUT SUT => (TSUT)CurrentTestProperties.Get(SutKey);

        /// <summary>
        /// Returns a new Logger everytime it is called
        /// </summary>
        protected override ILogger Logger => ResolveService<ILogger<TSUT>>();
        protected const string SutKey = "_sut";
    }
}
