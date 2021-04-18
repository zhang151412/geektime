using GeekTime.Domain;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GeekTime.Infrastructure.Core
{
    /// <summary>
    /// 包含普通实体的仓储
    /// 约束 TEntity 必须是继承 Entity 的基类，必须实现聚合根 IAggregateRoot
    /// 也就是说仓储里面存储的对象必须是一个聚合根对象
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    public interface IRepository<TEntity> where TEntity : Entity, IAggregateRoot
    {
        IUnitOfWork UnitOfWork { get; }
        TEntity Add(TEntity entity);
        Task<TEntity> AddAsync(TEntity entity, CancellationToken cancellationToken = default);
        TEntity Update(TEntity entity);
        Task<TEntity> UpdateAsync(TEntity entity, CancellationToken cancellationToken = default);
        bool Remove(Entity entity);// 由于没有指定主键，只能根据当前实体进行删除操作
        Task<bool> RemoveAsync(Entity entity);
    }

    /// <summary>
    /// 包含指定主键的类型的实体的仓储
    /// 继承了上面的接口 IRepository<TEntity>，也就是说拥有了上面定义的所有方法
    /// 另外一个，它实现了几个跟 Id 相关的操作的方法
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    /// <typeparam name="TKey"></typeparam>
    public interface IRepository<TEntity, TKey> : IRepository<TEntity> where TEntity : Entity<TKey>, IAggregateRoot
    {
        bool Delete(TKey id);
        Task<bool> DeleteAsync(TKey id, CancellationToken cancellationToken = default);
        TEntity Get(TKey id);
        Task<TEntity> GetAsync(TKey id, CancellationToken cancellationToken = default);
    }
}
