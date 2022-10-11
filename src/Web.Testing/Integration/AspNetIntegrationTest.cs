using Core.Plugins.NUnit.Integration;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Web.Testing.Integration
{
    public abstract class AspNetIntegrationTest : IntegrationTest
    {
        protected HttpRequest CreateHttpRequest()
        {
            var httpRequest = new DefaultHttpContext().Request;

            httpRequest.Headers["X-Username"] = TestUsername;

            return httpRequest;
        }

        protected HttpRequest CreateHttpRequest(string key, string val)
        {
            var queryStringParameters = new Dictionary<string, string>
            {
                { key, val }
            };

            return CreateHttpRequest(queryStringParameters);
        }

        protected HttpRequest CreateHttpRequest(Dictionary<string, string> queryStringParameters)
        {
            HttpRequest request = CreateHttpRequest();

            request.QueryString = QueryString.Create(queryStringParameters);

            return request;
        }

        protected HttpRequest CreateHttpRequestWithBody(object body)
        {
            HttpRequest httpRequest = CreateHttpRequest();

            string bodyAsJson = JsonConvert.SerializeObject(body);

            httpRequest.Body = new MemoryStream(Encoding.ASCII.GetBytes(bodyAsJson));

            return httpRequest;
        }
    }

    public abstract class AspNetIntegrationTest<TSUT> : AspNetIntegrationTest
    {
        protected TSUT SUT => (TSUT)CurrentTestProperties.Get(SutKey);
        //protected override ILogger Logger => ResolveService<ILogger<TSUT>>();

        protected override void BootstrapTest()
        {
            //base.BootstrapTest();

            //var serviceProvider = (IServiceProvider)CurrentTestProperties.Get(ServiceProviderKey);

            //TSUT sut = serviceProvider.GetRequiredService<TSUT>();

            //CurrentTestProperties.Set(SutKey, sut);
        }

        protected const string SutKey = "_sut";
    }
}
