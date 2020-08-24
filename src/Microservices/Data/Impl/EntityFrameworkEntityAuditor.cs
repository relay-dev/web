using Core.Providers;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;

namespace Microservices.Data.Impl
{
    public class EntityFrameworkEntityAuditor : IEntityAuditor
    {
        private readonly IUsernameProvider _usernameProvider;
        private readonly IDateTimeProvider _dateTimeProvider;
        private readonly ILogger<EntityFrameworkEntityAuditor> _logger;

        public EntityFrameworkEntityAuditor(
            IUsernameProvider usernameProvider,
            IDateTimeProvider dateTimeProvider,
            ILogger<EntityFrameworkEntityAuditor> logger)
        {
            _usernameProvider = usernameProvider;
            _dateTimeProvider = dateTimeProvider;
            _logger = logger;
        }

        public void Audit(IEnumerable<EntityEntry> entities)
        {
            foreach (EntityEntry entry in entities)
            {
                string username = _usernameProvider.Get();
                DateTime serverDateTime = _dateTimeProvider.Get();

                switch (entry.State)
                {
                    case EntityState.Added:
                        try
                        {
                            ((dynamic)entry.Entity).CreatedBy = username;
                            ((dynamic)entry.Entity).CreatedDate = serverDateTime;
                        }
                        catch (Exception)
                        {
                            _logger.LogWarning($"Could not audit entity without Created fields. Type: {entry.Entity.GetType().Namespace}");
                        }
                        break;
                    case EntityState.Modified:
                        try
                        {
                            ((dynamic)entry.Entity).ModifiedBy = username;
                            ((dynamic)entry.Entity).ModifiedDate = serverDateTime;
                        }
                        catch (Exception)
                        {
                            _logger.LogWarning($"Could not audit entity without Modified fields. Type: {entry.Entity.GetType().Namespace}");
                        }
                        break;
                }
            }
        }
    }
}
