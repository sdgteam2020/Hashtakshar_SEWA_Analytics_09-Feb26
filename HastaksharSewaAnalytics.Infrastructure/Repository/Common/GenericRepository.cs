using HastaksharSewaAnalytics.Application.Abstractions.Interfaces.Common;
using HastaksharSewaAnalytics.Domain.Premitives.Entity;
using HastaksharSewaAnalytics.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace HastaksharSewaAnalytics.Infrastructure.Repository.Common;

public sealed record GenericRepository<TEntity, TId> : IGenericRepository<TEntity, TId>
    where TEntity : Entity<TId>
    where TId : notnull
{
    private readonly HastaksharSewaAnalyticsDbContext _context;
    private readonly DbSet<TEntity> _set;

    public GenericRepository(HastaksharSewaAnalyticsDbContext context)
    {
        _context = context;
        _set = context.Set<TEntity>();
    }

    public async Task AddAsync(TEntity entity, CancellationToken ct = default)
        => await _set.AddAsync(entity, ct);

    public async Task<TEntity?> GetByIdAsync(TId id, CancellationToken ct = default)
        => await _set.FindAsync(new object?[] { id }, ct);

    public void Update(TEntity entity)
        => _set.Update(entity);

    public void Remove(TEntity entity)
        => _set.Remove(entity);


    public async Task<int> CountAsync(
        Expression<Func<TEntity, bool>>? predicate = null,
        CancellationToken ct = default)
    {
        return predicate == null
            ? await _set.CountAsync(ct)
            : await _set.CountAsync(predicate, ct);
    }

    public async Task<List<TResult>> GetListAsync<TResult>(
        Expression<Func<TEntity, TResult>> selector,
        Expression<Func<TEntity, bool>>? predicate = null,
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null,
        CancellationToken cancellationToken = default)
    {
        IQueryable<TEntity> query = _set.AsNoTracking();
         
        if (predicate != null)
        {
            query = query.Where(predicate);
        }
         
        if (orderBy != null)
        {
            query = orderBy(query);
        }
         
        return await query.Select(selector).ToListAsync(cancellationToken);
    }

    public async Task<TResult?> GetScalarAsync<TResult>(
    Expression<Func<TEntity, TResult>> selector,
    Expression<Func<TEntity, bool>>? predicate = null,
    Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null,
    CancellationToken cancellationToken = default)
    {
        IQueryable<TEntity> query = _set.AsNoTracking();

        if (predicate != null)
            query = query.Where(predicate);

        if (orderBy != null)
            query = orderBy(query);

        return await query.Select(selector).FirstOrDefaultAsync(cancellationToken);
    }

    public IQueryable<TEntity> Query()
    => _context.Set<TEntity>().AsNoTracking();

}
