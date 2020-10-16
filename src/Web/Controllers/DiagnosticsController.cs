using Core.Application;
using Core.Caching;
using Core.Providers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data.Common;

namespace Web.Controllers
{
    [ApiController]
    [Route("v1/[controller]")]
    public class DiagnosticsController<TDbContext> : Controller where TDbContext : DbContext
    {
        private readonly ICache _cache;
        private readonly TDbContext _dbContext;
        private readonly ILogger<DiagnosticsController<TDbContext>> _logger;
        private readonly IApplicationContextProvider _applicationContextProvider;

        public DiagnosticsController(
            ICache cache,
            TDbContext dbContext,
            ILogger<DiagnosticsController<TDbContext>> logger,
            IApplicationContextProvider applicationContextProvider)
        {
            _cache = cache;
            _dbContext = dbContext;
            _logger = logger;
            _applicationContextProvider = applicationContextProvider;
        }

        [HttpGet]
        public ActionResult<ApplicationDiagnostics> Get()
        {
            ApplicationContext applicationContext = _applicationContextProvider.Get();

            var diagnostics = new ApplicationDiagnostics
            {
                ApplicationId = applicationContext.ApplicationId,
                ApplicationName = applicationContext.ApplicationName,
                ApplicationVersion = applicationContext.ApplicationVersion,
                BuildTimestamp = applicationContext.BuildTimestamp
            };

            return new OkObjectResult(diagnostics);
        }

        [HttpGet]
        [Route("Cache/Clear")]
        public ActionResult ClearCache()
        {
            List<string> cacheKeys = _cache.GetAllKeys();

            _cache.RemoveAll();

            return new OkObjectResult($"Cleared cache entries for: '{string.Join("', '", cacheKeys)}'");
        }

        [HttpGet]
        [Route("Database")]
        public ActionResult<string> Database()
        {
            DbConnection connection = _dbContext.Database.GetDbConnection();

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
    }
}
