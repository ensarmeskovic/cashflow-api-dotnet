using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cashflow.Infrastructure.Repositories
{
    public interface IRepository<TEntity, in TPk> where TEntity : class
    {
        void Add(TEntity entity);
        void AddRange(IQueryable<TEntity> entities);

        TEntity Remove(TEntity entity, bool softDelete = true);
        TEntity RemoveById(int id, bool softDelete = true);

        IEnumerable<TEntity> RemoveRange(IQueryable<TEntity> entities, bool softDelete = true);
        IEnumerable<TEntity> RemoveRangeByIds(int[] ids, bool softDelete = true);

        void Update(TEntity entity);
        void UpdateRange(IQueryable<TEntity> entities);


        TEntity GetById(TPk id);
        Task<TEntity> GetByIdAsync(TPk id);


        IEnumerable<TEntity> GetAll();
    }
}