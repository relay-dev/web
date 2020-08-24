using Core.Framework;
using Core.Providers;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Collections.Generic;

namespace Microservices.Data.Impl
{
    public class EntityFrameworkEntityAuditor : IEntityAuditor
    {
        private readonly IUsernameProvider _usernameProvider;
        private readonly IDateTimeProvider _dateTimeProvider;

        public EntityFrameworkEntityAuditor(IUsernameProvider usernameProvider, IDateTimeProvider dateTimeProvider)
        {
            _usernameProvider = usernameProvider;
            _dateTimeProvider = dateTimeProvider;
        }

        public void Audit(IEnumerable<EntityEntry> entities)
        {
            foreach (EntityEntry entry in entities)
            {
                string username = _usernameProvider.Get();
                DateTime serverDateTime = _dateTimeProvider.Get();

                if (entry.State == EntityState.Added)
                {
                    if (entry.Entity is IHaveCreatedFields entryWithCreatedFields)
                    {
                        entryWithCreatedFields.CreatedBy = username;
                        entryWithCreatedFields.CreatedDate = serverDateTime;
                    }
                }
                else
                {
                    if (entry.Entity is IHaveModifiedFields entryWithModifiedFields)
                    {
                        entryWithModifiedFields.ModifiedBy = username;
                        entryWithModifiedFields.ModifiedDate = serverDateTime;
                    }
                }
            }
        }
    }
}
