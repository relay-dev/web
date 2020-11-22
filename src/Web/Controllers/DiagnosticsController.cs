﻿using Core.Application;
using Core.Plugins.Caching;
using Core.Plugins.Framework;
using Core.Providers;
using Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;
using Web.Providers;

namespace Web.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class DiagnosticsController : Controller
    {
        private readonly IMemoryCache _cache;
        private readonly IConfiguration _configuration;
        private readonly IDbContextProvider _dbContextProvider;
        private readonly ApplicationContext _applicationContext;
        private readonly WarmupTaskExecutor _warmupTaskExecutor;
        private readonly ILogger<DiagnosticsController> _logger;
        private readonly IConnectionStringParser _connectionStringParser;
        private readonly IConnectionStringProvider _connectionStringProvider;

        public DiagnosticsController(
            IMemoryCache cache,
            IConfiguration configuration,
            IDbContextProvider dbContextProvider,
            ApplicationContext applicationContext,
            WarmupTaskExecutor warmupTaskExecutor,
            ILogger<DiagnosticsController> logger,
            IConnectionStringParser connectionStringParser, 
            IConnectionStringProvider connectionStringProvider)
        {
            _cache = cache;
            _configuration = configuration;
            _dbContextProvider = dbContextProvider;
            _applicationContext = applicationContext;
            _warmupTaskExecutor = warmupTaskExecutor;
            _logger = logger;
            _connectionStringParser = connectionStringParser;
            _connectionStringProvider = connectionStringProvider;
        }

        [HttpGet]
        public ActionResult<ApplicationDiagnostics> Get()
        {
            var diagnostics = new ApplicationDiagnostics
            {
                ApplicationId = _applicationContext.ApplicationId,
                ApplicationName = _applicationContext.ApplicationName,
                ApplicationVersion = _applicationContext.ApplicationVersion,
                BuildTimestamp = _applicationContext.BuildTimestamp
            };

            return new OkObjectResult(diagnostics);
        }

        [HttpGet]
        [Route("Cache")]
        public ActionResult Cache()
        {
            var entries = new List<string>();

            foreach (string key in CacheKeyManager.GetAllKeys())
            {
                entries.Add($"{key}: {_cache.Get(key)}");
            }

            string message = $"Cache entries: {Environment.NewLine}{Environment.NewLine}{string.Join(Environment.NewLine, entries)}";

            return new OkObjectResult(message);
        }

        [HttpGet]
        [Route("Cache/Clear")]
        public ActionResult ClearCache()
        {
            List<string> cacheKeys = CacheKeyManager.GetAllKeys();

            _cache.Clear();

            string message = $"Cleared cache entries for: {Environment.NewLine}{Environment.NewLine}'{string.Join(Environment.NewLine, cacheKeys)}'";

            return new OkObjectResult(message);
        }

        [HttpGet]
        [Route("Configuration")]
        public ActionResult Configuration()
        {
            var entries = new List<string>();

            foreach (KeyValuePair<string, string> kvp in _configuration.AsEnumerable())
            {
                entries.Add($"{kvp.Key}: {kvp.Value}");
            }

            string message = $"Configuration entries: {Environment.NewLine}{Environment.NewLine}{string.Join(Environment.NewLine, entries)}";

            return new OkObjectResult(message);
        }

        [HttpGet]
        [Route("Database")]
        public ActionResult<string> Database()
        {
            var diagnostics = new DatabaseDiagnostics();

            DbContext dbContext = _dbContextProvider.Get();

            DbConnection connection = dbContext?.Database.GetDbConnection();

            if (connection != null)
            {
                diagnostics.ServerName = connection.DataSource;
                diagnostics.DatabaseName = connection.Database;
            }

            return new OkObjectResult(diagnostics);
        }

        [HttpGet]
        [Route("Log")]
        public ActionResult<string> Log(string message)
        {
            message ??= Guid.NewGuid().ToString();

            _logger.LogInformation(new EventId(), message);

            return new OkObjectResult($"Log Message: {message}");
        }

        [HttpGet]
        [Route("Warmup")]
        public ActionResult Warmup()
        {
            Task.Factory.StartNew(() => _warmupTaskExecutor.RunAsync(new CancellationToken()));

            return new OkObjectResult("Complete");
        }

        [HttpGet]
        [Route("DefaultConnection")]
        public ActionResult<string> DefaultConnection()
        {
            Dictionary<string, string> connectionStringSegments = GetConnectionStringSegments("DefaultConnection");

            if (connectionStringSegments == null)
            {
                return new NotFoundResult();
            }

            var data = new
            {
                DataSource = connectionStringSegments.TryGetValueOrNull("Data Source"),
                InitialCatalog = connectionStringSegments.TryGetValueOrNull("Initial Catalog")
            };

            return new OkObjectResult(data);
        }

        [HttpGet]
        [Route("DefaultStorageConnection")]
        public ActionResult<string> DefaultStorageConnection()
        {
            Dictionary<string, string> connectionStringSegments = GetConnectionStringSegments("DefaultStorageConnection");

            if (connectionStringSegments == null)
            {
                return new NotFoundResult();
            }

            var data = new
            {
                AccountName = connectionStringSegments.TryGetValueOrNull("AccountName")
            };

            return new OkObjectResult(data);
        }

        [HttpGet]
        [Route("DefaultFtpConnection")]
        public ActionResult<string> DefaultFtpConnection()
        {
            Dictionary<string, string> connectionStringSegments = GetConnectionStringSegments("DefaultFtpConnection");

            if (connectionStringSegments == null)
            {
                return new NotFoundResult();
            }

            var data = new
            {
                Host = connectionStringSegments.TryGetValueOrNull("Host"),
                Username = connectionStringSegments.TryGetValueOrNull("Username"),
                IsSftp = connectionStringSegments.TryGetValueOrNull("IsSftp")
            };

            return new OkObjectResult(data);
        }

        [HttpGet]
        [Route("DefaultEventGridConnection")]
        public ActionResult<string> DefaultEventGridConnection()
        {
            Dictionary<string, string> connectionStringSegments = GetConnectionStringSegments("DefaultEventGridConnection");

            if (connectionStringSegments == null)
            {
                return new NotFoundResult();
            }

            var data = new
            {
                Endpoint = connectionStringSegments.TryGetValueOrNull("Endpoint"),
                Topic = connectionStringSegments.TryGetValueOrNull("Topic")
            };

            return new OkObjectResult(data);
        }

        private Dictionary<string, string> GetConnectionStringSegments(string connectionName)
        {
            string connectionString = _connectionStringProvider.GetOptional(connectionName);

            if (connectionString == null)
            {
                return null;
            }

            return _connectionStringParser.Parse(connectionString);
        }
    }
}
