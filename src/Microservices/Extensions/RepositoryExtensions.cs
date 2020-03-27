using Arch.EntityFrameworkCore.UnitOfWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Microservices.Extensions
{
    public static class RepositoryExtensions
    {
        public static TEntity Single<TEntity>(this IRepository<TEntity> repository, Expression<Func<TEntity, bool>> predicate = null) where TEntity : class
        {
            if (predicate == null)
            {
                return repository.GetPagedList().Items.Single();
            }
            else
            {
                return repository.GetPagedList(predicate: predicate).Items.Single();
            }
        }

        public static TEntity SingleOrDefault<TEntity>(this IRepository<TEntity> repository, Expression<Func<TEntity, bool>> predicate = null) where TEntity : class
        {
            if (predicate == null)
            {
                return repository.GetPagedList().Items.SingleOrDefault();
            }
            else
            {
                return repository.GetPagedList(predicate: predicate).Items.SingleOrDefault();
            }
        }

        public static TEntity First<TEntity>(this IRepository<TEntity> repository, Expression<Func<TEntity, bool>> predicate = null) where TEntity : class
        {
            if (predicate == null)
            {
                TEntity entity = repository.GetFirstOrDefault();

                if (entity == null)
                    throw new Exception();

                return entity;
            }
            else
            {
                TEntity entity = repository.GetFirstOrDefault(predicate: predicate);

                if (entity == null)
                    throw new Exception();

                return entity;
            }
        }

        public static TEntity FirstOrDefault<TEntity>(this IRepository<TEntity> repository, Expression<Func<TEntity, bool>> predicate = null) where TEntity : class
        {
            if (predicate == null)
            {
                return repository.GetFirstOrDefault();
            }
            else
            {
                return repository.GetFirstOrDefault(predicate: predicate);
            }
        }

        public static List<TEntity> ToList<TEntity>(this IRepository<TEntity> repository, Expression<Func<TEntity, bool>> predicate = null) where TEntity : class
        {
            if (predicate == null)
            {
                return repository.GetPagedList().Items.ToList();
            }
            else
            {
                return repository.GetPagedList(predicate: predicate).Items.ToList();
            }
        }
    }
}
