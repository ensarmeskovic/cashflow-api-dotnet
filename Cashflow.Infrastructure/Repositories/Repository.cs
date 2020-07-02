using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cashflow.Domain;
using Microsoft.EntityFrameworkCore;

namespace Cashflow.Infrastructure.Repositories
{
    public abstract class Repository<TEntity, TPk> : IRepository<TEntity, TPk> where TEntity : class
    {
        protected readonly CashflowContext Context;
        private readonly DbSet<TEntity> _entity;

        protected Repository(CashflowContext context)
        {
            Context = context;
            _entity = Context.Set<TEntity>();
        }


        #region Select
        public virtual TEntity GetById(TPk id) => _entity.AsNoTracking().SingleOrDefault(i => Equals(((IEntity)i).Id, id));
        public virtual async Task<TEntity> GetByIdAsync(TPk id) => await _entity.AsNoTracking().SingleOrDefaultAsync(i => Equals(((IEntity)i).Id, id));

        public virtual IEnumerable<TEntity> GetAll() => _entity.Where(i => !((IEntity)i).DeletedDateTime.HasValue);
        #endregion

        #region Add
        public virtual void Add(TEntity entity)
        {
            _entity.Add(entity);
        }

        public virtual void AddRange(IQueryable<TEntity> entities)
        {
            _entity.AddRange(entities);
        }
        #endregion

        #region Update
        public virtual void Update(TEntity entity)
        {
            _entity.Update(entity);
        }

        public void UpdateRange(IQueryable<TEntity> entities)
        {
            _entity.UpdateRange(entities);
        }
        #endregion

         #region Delete
        public virtual TEntity Remove(TEntity entity, bool softDelete = true)
        {
            if (softDelete)
            {
                ((IEntity)entity).DeletedDateTime = DateTime.Now;
                _entity.Update(entity);
            }
            else
            {
                _entity.Remove(entity);
            }

            return entity;
        }

        public virtual TEntity RemoveById(int id, bool softDelete = false)
        {
            TEntity entity = _entity.FirstOrDefault(i => ((IEntity)i).Id == id);
            if (entity is null)
                throw new ArgumentNullException($"Can not find {typeof(TEntity)} for id: {id}");

            if (softDelete)
            {
                ((IEntity)entity).DeletedDateTime = DateTime.Now;
                _entity.Update(entity);
            }
            else
            {
                _entity.Remove(entity);
            }

            return entity;
        }

        public virtual IEnumerable<TEntity> RemoveRange(IQueryable<TEntity> entities, bool softDelete = true)
        {
            if (softDelete)
            {
                foreach (TEntity entity in entities)
                    ((IEntity)entity).DeletedDateTime = DateTime.Now;

                _entity.UpdateRange(entities);
            }
            else
            {
                _entity.RemoveRange(entities);
            }

            return entities;
        }

        public virtual IEnumerable<TEntity> RemoveRangeByIds(int[] ids, bool softDelete = true)
        {
            IQueryable<TEntity> entities = _entity.Where(i => ids.Contains(((IEntity)i).Id));
            if (entities is null)
                throw new ArgumentNullException($"Can not find {typeof(TEntity)} for ids: {string.Join(", ", ids.ToArray())}");

            if (softDelete)
            {
                foreach (TEntity entity in entities)
                    ((IEntity)entity).DeletedDateTime = DateTime.Now;

                _entity.UpdateRange(entities);
            }
            else
            {
                _entity.RemoveRange(entities);
            }

            return entities;
        }
        #endregion
    }
}