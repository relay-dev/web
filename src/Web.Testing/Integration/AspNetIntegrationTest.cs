using Core.Plugins.NUnit.Integration;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Text;

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
}
