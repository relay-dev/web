using Core.Plugins.NUnit.Integration;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;

namespace Web.Testing.Integration
{
    public abstract class WebIntegrationTest<TToTest> : IntegrationTest<TToTest>
    {
        protected IHostBuilder CreateTestHostBuilder<TStartup>(string basePath = null) where TStartup : class =>
            Microsoft.Extensions.Hosting.Host.CreateDefaultBuilder(new string[0])
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<TStartup>();
                    webBuilder.ConfigureLogging((hostingContext, logging) =>
                    {
                        logging.AddConsole();
                        logging.AddDebug();
                    });
                })
                .ConfigureAppConfiguration((webBuilder, configBuilder) =>
                {
                    basePath ??= Path.Combine(SubstringBefore(AppDomain.CurrentDomain.BaseDirectory, "tests"), "src", typeof(TStartup).Namespace);

                    configBuilder
                        .SetBasePath(basePath)
                        .AddJsonFile("appsettings.json", true, true)
                        .AddJsonFile("appsettings.Development.json", true, true)
                        .AddJsonFile("local.settings.json", true, true)
                        .AddUserSecrets<TStartup>()
                        .AddEnvironmentVariables();
                });

        protected HttpRequest CreateHttpRequest()
        {
            return new DefaultHttpContext().Request;
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

        private static string SubstringBefore(string str, string removeAfter, bool includeRemoveAfterString = false)
        {
            if (str == null)
            {
                return null;
            }

            try
            {
                return str.Substring(0, str.IndexOf(removeAfter, StringComparison.Ordinal) + (includeRemoveAfterString ? 1 : 0));
            }
            catch
            {
                return str;
            }
        }
    }
}
