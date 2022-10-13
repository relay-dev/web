using Core.Plugins.NUnit.Integration;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Web.Testing.Integration
{
    /// <summary>
    /// A test fixture that contains tests which should run against real dependenies in a local or development web environment.
    /// This will run tests in a way that replicates the way the application will be when it's deployed by setting up an IHost to run the tests against.
    /// </summary>
    public abstract class WebIntegrationTest : IntegrationTest
    {
        /// <summary>
        /// Creates an HttpRequest
        /// </summary>
        protected virtual HttpRequest CreateHttpRequest()
        {
            return new DefaultHttpContext().Request;
        }

        /// <summary>
        /// Creates an HttpRequest with query string parameters
        /// </summary>
        protected virtual HttpRequest CreateHttpRequest(Dictionary<string, string> queryStringParameters)
        {
            HttpRequest request = CreateHttpRequest();

            request.QueryString = QueryString.Create(queryStringParameters);

            return request;
        }

        /// <summary>
        /// Creates an HttpRequest with a body as an object
        /// </summary>
        protected virtual HttpRequest CreateHttpRequestWithBody(object body)
        {
            string bodyAsJson = JsonConvert.SerializeObject(body);

            return CreateHttpRequestWithBody(bodyAsJson);
        }

        /// <summary>
        /// Creates an HttpRequest with a body as a string
        /// </summary>
        protected virtual HttpRequest CreateHttpRequestWithBody(string body)
        {
            HttpRequest httpRequest = CreateHttpRequest();

            httpRequest.Body = new MemoryStream(Encoding.ASCII.GetBytes(body));

            return httpRequest;
        }
    }

    /// <summary>
    /// A test fixture that contains tests which should run against real dependenies in a local or development web environment where you want to speficy the type to test.
    /// This will run tests in a way that replicates the way the application will be when it's deployed by setting up an IHost to run the tests against.
    /// </summary>
    public abstract class WebIntegrationTest<TSUT> : WebIntegrationTest
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
