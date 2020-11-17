using Core.Plugins.NUnit.Integration;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;

namespace Web.Testing.Integration
{
    public abstract class AspNetIntegrationTest<TToTest> : IntegrationTest<TToTest>
    {
        protected HttpRequest CreateHttpRequest()
        {
            var httpRequest = new DefaultHttpContext().Request;

            httpRequest.Headers["X-Username"] = TestUsername;

            return httpRequest;
        }

        protected HttpRequest CreateHttpRequest(string key, string val)
        {
            HttpRequest request = CreateHttpRequest();

            request.QueryString = QueryString.Create(
                new Dictionary<string, string>
                {
                    {key, val}
                });

            return request;
        }

        protected HttpRequest CreateHttpRequest(Dictionary<string, string> queryStringParameters)
        {
            HttpRequest request = CreateHttpRequest();

            request.QueryString = QueryString.Create(queryStringParameters);

            return request;
        }
    }
}
