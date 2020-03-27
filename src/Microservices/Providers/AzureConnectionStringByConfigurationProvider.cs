using Core.Plugins.Extensions;
using Core.Plugins.Providers;
using Microservices.Exceptions;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;

namespace Microservices.Providers
{
    public class AzureConnectionStringByConfigurationProvider : ConnectionStringProviderBase
    {
        private readonly IConfiguration _configuration;

        public AzureConnectionStringByConfigurationProvider(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        protected override string GetConnectionString(string connectionName)
        {
            string connectionString = _configuration.GetConnectionString(connectionName);

            if (connectionString == null)
                throw new MicroserviceException(HttpStatusCode.InternalServerError, $"ConnectionName '{connectionName}' not found");

            if (connectionString == string.Empty)
                throw new MicroserviceException(HttpStatusCode.InternalServerError, $"ConnectionName '{connectionName}' cannot be an empty string");

            if (!connectionString.Contains("{{"))
            {
                return connectionString;
            }

            var regex = new Regex(@"{{(.*?)}}", RegexOptions.Compiled | RegexOptions.IgnoreCase);

            Dictionary<string, string> connectionStringVariables = regex.Matches(connectionString)
                .Select(match => match.ToString())
                .OrderBy(s => s)
                .ToDictionary(s => s, s => s.Remove("{{").Remove("}}"));

            foreach (KeyValuePair<string, string> connectionStringVariable in connectionStringVariables)
            {
                if (connectionString.Contains(connectionStringVariable.Key))
                {
                    connectionString = connectionString.Replace(connectionStringVariable.Key, _configuration[connectionStringVariable.Value]);
                }
            }

            return connectionString;
        }
    }
}
