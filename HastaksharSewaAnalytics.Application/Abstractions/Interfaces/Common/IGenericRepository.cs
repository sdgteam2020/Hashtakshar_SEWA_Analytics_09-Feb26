using HastaksharSewaAnalytics.Domain.Premitives.Entity;
using System.Linq.Expressions;

namespace HastaksharSewaAnalytics.Application.Abstractions.Interfaces.Common;

public interface IGenericRepository<TEntity, TId>
    where TEntity : Entity<TId>
    where TId : notnull
{
    Task<TEntity?> GetByIdAsync(
        TId id, 
        CancellationToken ct = default);
    Task AddAsync(
        TEntity entity, 
        CancellationToken ct = default);
    void Update(TEntity entity);
    void Remove(TEntity entity);
    IQueryable<TEntity> Query();
    Task<int> CountAsync(
        Expression<Func<TEntity, bool>>? predicate = null,
        CancellationToken ct = default);
    Task<List<TResult>> GetListAsync<TResult>(
          Expression<Func<TEntity, TResult>> selector,
          Expression<Func<TEntity, bool>>? predicate = null,
          Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null,
          CancellationToken cancellationToken = default
      );
    Task<TResult?> GetScalarAsync<TResult>(
           Expression<Func<TEntity, TResult>> selector,
           Expression<Func<TEntity, bool>>? predicate = null,
           Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null,
           CancellationToken cancellationToken = default);

}

