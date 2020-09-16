using Core.Plugins.NUnit.Integration;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;

namespace Web.Testing.Unit
{
    public abstract class InMemoryDbContextTest<TSUT, TDbContext> : IntegrationTest<TSUT>, IDisposable where TDbContext : DbContext, new()
    {
        private readonly Func<DbContextOptions, TDbContext> _dbContextInitValueFactory;
        private readonly List<TDbContext> _dbContextsCreated;

        protected InMemoryDbContextTest(Func<DbContextOptions, TDbContext> dbContextInitValueFactory)
        {
            _dbContextInitValueFactory = dbContextInitValueFactory;
            _dbContextsCreated = new List<TDbContext>();
        }

        public override void Setup()
        {
            var options = new DbContextOptionsBuilder<TDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            TDbContext dbContext = _dbContextInitValueFactory.Invoke(options);

            dbContext.Database.EnsureCreated();

            //TSUT sut = (TSUT)CurrentTestProperties.Get("_sut");
            //var serviceProvider = (IServiceProvider)CurrentTestProperties.Get("_serviceProvider");

            CurrentTestProperties.Set(DbContextKey, dbContext);

            _dbContextsCreated.Add(dbContext);

            base.Setup();
        }

        protected TDbContext ResolveDbContext()
        {
            return (TDbContext)CurrentTestProperties.Get(DbContextKey);
        }

        public void Dispose()
        {
            foreach (TDbContext dbContext in _dbContextsCreated)
            {
                dbContext.Database.EnsureDeleted();

                dbContext.Dispose();
            }
        }

        protected abstract void Seed(TDbContext dbContext);
        private const string DbContextKey = "_dbContext";
    }

    public abstract class InMemoryDbContextTest<TDbContext> : IntegrationTest, IDisposable where TDbContext : DbContext, new()
    {
        protected readonly TDbContext DbContext;

        protected InMemoryDbContextTest(Func<DbContextOptions, TDbContext> dbContextInitValueFactory)
        {
            var options = new DbContextOptionsBuilder<TDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            DbContext = dbContextInitValueFactory.Invoke(options);

            DbContext.Database.EnsureCreated();
        }

        public void Dispose()
        {
            DbContext.Database.EnsureDeleted();

            DbContext.Dispose();
        }

        protected abstract void Seed(TDbContext dbContext);
    }
}
