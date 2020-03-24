using Arch.EntityFrameworkCore.UnitOfWork;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Moq;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Microservices.Testing.Unit
{
    public class FakeRepository<TEntity> : Repository<TEntity> where TEntity : class
    {
        public List<TEntity> TestData { get; set; }

        public FakeRepository()
            : base(Mock.Of<DbContext>())
        {
            TestData = new List<TEntity>();
        }

        public FakeRepository(List<TEntity> entities)
            : base(Mock.Of<DbContext>())
        {
            TestData = entities;
        }

        public override void Insert(TEntity entity)
        {
            TestData.Add(entity);
        }

        public override void Insert(params TEntity[] entities)
        {
            TestData.AddRange(entities);
        }

        public override void Insert(IEnumerable<TEntity> entities)
        {
            TestData.AddRange(entities);
        }

        public override ValueTask<EntityEntry<TEntity>> InsertAsync(TEntity entity, CancellationToken cancellationToken = default)
        {
            TestData.Add(entity);

            return base.InsertAsync(entity, cancellationToken);
        }

        public override Task InsertAsync(params TEntity[] entities)
        {
            TestData.AddRange(entities);

            return base.InsertAsync(entities);
        }

        public override Task InsertAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default)
        {
            TestData.AddRange(entities);

            return base.InsertAsync(entities, cancellationToken);
        }
    }
}
