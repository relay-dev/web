using Microsoft.EntityFrameworkCore;

namespace Web.Providers
{
    public class DbContextProvider : IDbContextProvider
    {
        private readonly DbContext _dbContext;

        public DbContextProvider(DbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public DbContext Get()
        {
            return _dbContext;
        }
    }

    public interface IDbContextProvider
    {
        DbContext Get();
    }
}
