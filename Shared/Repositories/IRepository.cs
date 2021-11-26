using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Shared.Repositories
{
    public interface IRepository<TEntity> where TEntity : class
    {
        // Create
        Task<TEntity> AddAsync(TEntity entity);
        Task AddRangeAsync(IEnumerable<TEntity> entities);

        // Read
        Task<IEnumerable<TEntity>> GetAllAsync();

        [return: MaybeNull]
        Task<IEnumerable<TEntity>> FindAsync(Expression<Func<TEntity, bool>> predicate);

        Task<TEntity> SingleOrDefaultAsync(Expression<Func<TEntity, bool>> predicate);

        // Delete
        void Remove(TEntity entity);
        void RemoveRange(IEnumerable<TEntity> entities);
    }
}