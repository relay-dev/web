﻿using Core.Plugins.Providers;
using Microservices.Exceptions;
using Microsoft.Extensions.Configuration;
using System.Net;

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

            if (connectionString.Contains("{{DatabaseUsername}}"))
            {
                connectionString = connectionString.Replace("{{DatabaseUsername}}", _configuration["DatabaseUsername"]);
            }

            if (connectionString.Contains("{{DatabasePassword}}"))
            {
                connectionString = connectionString.Replace("{{DatabasePassword}}", _configuration["DatabasePassword"]);
            }

            return connectionString;
        }
    }
}
