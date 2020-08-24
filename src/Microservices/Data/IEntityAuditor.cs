using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.Collections.Generic;

namespace Microservices.Data
{
    public interface IEntityAuditor
    {
        void Audit(IEnumerable<EntityEntry> entities);
    }
}
