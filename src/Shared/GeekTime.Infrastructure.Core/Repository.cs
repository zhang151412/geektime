using GeekTime.Domain;
using System.Threading;
using System.Threading.Tasks;

namespace GeekTime.Infrastructure.Core
{
    /// <summary>
    /// 定义普通实体的仓储
    /// 定义约束 TDbContext 必须是 EFContext，也就是仓储必须依赖于 EFContext 及其子类
    /// 将来就可以把自己定义的比如 DomainContext 作为泛型参数传入 Repository，就可以很快捷地定义出来自己的仓储
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    /// <typeparam name="TDbContext"></typeparam>
    public abstract class Repository<TEntity, TDbContext> : IRepository<TEntity> where TEntity : Entity, IAggregateRoot where TDbContext : EFContext
    {
        // 具体实现需要依赖 DbContext
        protected virtual TDbContext DbContext { get; set; }

        public Repository(TDbContext context)
        {
            this.DbContext = context;
        }
        public virtual IUnitOfWork UnitOfWork => DbContext;
        // 因为 DbContext， EFContext 实际上实现了 IUnitOfWork，所以直接返回

        // 下面这些方法都是 EntityFramework 提供的能力，所以就能通过简单的几行代码来实现基本的仓储操作
        public virtual TEntity Add(TEntity entity)
        {
            return DbContext.Add(entity).Entity;
        }

        public virtual Task<TEntity> AddAsync(TEntity entity, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(Add(entity));
        }

        public virtual TEntity Update(TEntity entity)
        {
            return DbContext.Update(entity).Entity;
        }

        public virtual Task<TEntity> UpdateAsync(TEntity entity, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(Update(entity));
        }

        public virtual bool Remove(Entity entity)
        {
            DbContext.Remove(entity);
            return true;
        }

        public virtual Task<bool> RemoveAsync(Entity entity)
        {
            return Task.FromResult(Remove(entity));
        }
    }

    /// <summary>
    /// 定义主键的实体的仓储
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TDbContext"></typeparam>
    public abstract class Repository<TEntity, TKey, TDbContext> : Repository<TEntity, TDbContext>, IRepository<TEntity, TKey> where TEntity : Entity<TKey>, IAggregateRoot where TDbContext : EFContext
    {
        public Repository(TDbContext context) : base(context)
        {
        }
        /// <summary>
        /// 根据 Id 从 DbContext 获取 Entity，然后再 Remove
        /// 这样的好处是可以跟踪对象的状态
        /// 坏处是任意的删除都需要先去数据库里面做查询
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public virtual bool Delete(TKey id)
        {
            var entity = DbContext.Find<TEntity>(id);
            if (entity == null)
            {
                return false;
            }
            DbContext.Remove(entity);
            return true;
        }

        public virtual async Task<bool> DeleteAsync(TKey id, CancellationToken cancellationToken = default)
        {
            var entity = await DbContext.FindAsync<TEntity>(id, cancellationToken);
            if (entity == null)
            {
                return false;
            }
            DbContext.Remove(entity);
            return true;
        }

        public virtual TEntity Get(TKey id)
        {
            return DbContext.Find<TEntity>(id);
        }

        public virtual async Task<TEntity> GetAsync(TKey id, CancellationToken cancellationToken = default)
        {
            return await DbContext.FindAsync<TEntity>(id, cancellationToken);
        }
    }



}
