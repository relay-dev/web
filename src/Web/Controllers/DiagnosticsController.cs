using Core.Application;
using Core.Plugins.Caching;
using Core.Plugins.Framework;
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

        public DiagnosticsController(
            IMemoryCache cache,
            IConfiguration configuration,
            IDbContextProvider dbContextProvider,
            ApplicationContext applicationContext,
            WarmupTaskExecutor warmupTaskExecutor,
            ILogger<DiagnosticsController> logger)
        {
            _cache = cache;
            _configuration = configuration;
            _dbContextProvider = dbContextProvider;
            _applicationContext = applicationContext;
            _warmupTaskExecutor = warmupTaskExecutor;
            _logger = logger;
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
            var configurations = new List<string>();

            foreach (KeyValuePair<string, string> kvp in _configuration.AsEnumerable())
            {
                configurations.Add($"{kvp.Key}: {kvp.Value}");
            }

            string message = $"Configuration entries: {Environment.NewLine}{Environment.NewLine}{string.Join(Environment.NewLine, configurations)}";

            return new OkObjectResult(message);
        }
        
        [HttpGet]
        [Route("Database")]
        public ActionResult<string> Database()
        {
            DbConnection connection = _dbContextProvider.Get().Database.GetDbConnection();

            var diagnostics = new DatabaseDiagnostics
            {
                ServerName = connection.DataSource,
                DatabaseName = connection.Database
            };

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
    }
}
