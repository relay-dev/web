using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using Web.Testing.Integration;
using ExecutionContext = Microsoft.Azure.WebJobs.ExecutionContext;

namespace Web.Testing.AzureFunctions.Integration
{
    /// <summary>
    /// A test fixture that contains tests which should run against real dependenies in a local or development Azure Function App.
    /// This will run tests in a way that replicates the way the application will be when it's deployed by setting up an IHost to run the tests against.
    /// </summary>
    public abstract class FunctionIntegrationTest : WebIntegrationTest
    {
        /// <summary>
        /// The execution context from the current thread
        /// </summary>
        protected virtual ExecutionContext ExecutionContext => new ExecutionContext();
    }

    /// <summary>
    /// A test fixture that contains tests which should run against real dependenies in a local or development Azure Function App where you want to speficy the type to test.
    /// This will run tests in a way that replicates the way the application will be when it's deployed by setting up an IHost to run the tests against.
    /// </summary>
    public abstract class FunctionIntegrationTest<TSUT> : FunctionIntegrationTest
    {
        public override void BootstrapTest()
        {
            base.BootstrapTest();

            // Get this test's service provider. It was set on the test's current context by the BootstrapTest() method
            var serviceProvider = (IServiceProvider)CurrentTestProperties.Get(ServiceProviderKey);

            // Use the service provider to resolve an instance of the type under test
            TSUT sut = serviceProvider.GetRequiredService<TSUT>();

            // Set the instance on this test's context so we can reference it in SUT
            CurrentTestProperties.Set(SutKey, sut);
        }

        /// <summary>
        /// Returns a new SUT everytime it is called
        /// </summary>
        /// <remarks>SUT stands for System Under Test. It's meant to convey that we are testing more than just a class. We're testing the system by executing end-to-end tests meant to mimic what happens in a deployed environment.</remarks>
        protected virtual TSUT SUT => (TSUT)CurrentTestProperties.Get(SutKey);

        /// <summary>
        /// Returns a new Logger everytime it is called
        /// </summary>
        protected override ILogger Logger => ResolveService<ILogger<TSUT>>();
        protected const string SutKey = "_sut";
    }
}
