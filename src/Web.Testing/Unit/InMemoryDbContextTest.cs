using Core.Plugins.NUnit;
using Core.Plugins.NUnit.Unit;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;

namespace Web.Testing.Unit
{
    public abstract class InMemoryDbContextTest<TCUT, TDbContext> : AutoMockTest<TCUT>, IDisposable
        where TDbContext : DbContext
        where TCUT : class,
        new()
    {
        private readonly Func<DbContextOptions, TDbContext> _dbContextInitValueFactory;
        private readonly List<TDbContext> _dbContextsCreated;

        protected InMemoryDbContextTest(Func<DbContextOptions, TDbContext> dbContextInitValueFactory)
        {
            _dbContextInitValueFactory = dbContextInitValueFactory;
            _dbContextsCreated = new List<TDbContext>();
        }

        public override void BootstrapTest()
        {
            base.BootstrapTest();

            // Build options an in-memory database for the TDbContext type
            var options = new DbContextOptionsBuilder<TDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            // Create the DbContext using the options
            TDbContext dbContext = _dbContextInitValueFactory.Invoke(options);

            // Create the database
            dbContext.Database.EnsureCreated();

            // Set the instance on this test's context so we can reference it in ResolveDbContext()
            CurrentTestProperties.Set(DbContextKey, dbContext);

            // Add to the list of created contexts so we know what to cleanup later
            _dbContextsCreated.Add(dbContext);
        }

        /// <summary>
        // Get this test's DbContext. It was set on the test's current context by the BootstrapTest() method.
        /// </summary>
        protected virtual TDbContext ResolveDbContext()
        {
            return (TDbContext)CurrentTestProperties.Get(DbContextKey);
        }

        public void Dispose()
        {
            foreach (TDbContext dbContext in _dbContextsCreated)
            {
                // Remove the database
                dbContext.Database.EnsureDeleted();

                dbContext.Dispose();
            }
        }

        /// <summary>
        /// Adds data to the in-memory database of the DbContext
        /// </summary>
        protected abstract void Seed(TDbContext dbContext);
        private const string DbContextKey = "_dbContext";
    }

    public abstract class InMemoryDbContextTest<TDbContext> : TestBase, IDisposable where TDbContext : DbContext, new()
    {
        protected readonly TDbContext DbContext;

        protected InMemoryDbContextTest(Func<DbContextOptions, TDbContext> dbContextInitValueFactory)
        {
            // Build options an in-memory database for the TDbContext type
            var options = new DbContextOptionsBuilder<TDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            // Create the DbContext using the options
            DbContext = dbContextInitValueFactory.Invoke(options);

            // Create the database
            DbContext.Database.EnsureCreated();
        }

        public void Dispose()
        {
            // Remove the database
            DbContext.Database.EnsureDeleted();

            DbContext.Dispose();
        }

        /// <summary>
        /// Adds data to the in-memory database of the DbContext
        /// </summary>
        protected abstract void Seed(TDbContext dbContext);
    }
}
