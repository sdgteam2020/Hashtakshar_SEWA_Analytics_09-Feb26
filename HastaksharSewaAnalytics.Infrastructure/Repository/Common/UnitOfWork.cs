using HastaksharSewaAnalytics.Application.Abstractions.Interfaces.Common;
using HastaksharSewaAnalytics.Domain.Premitives.Entity;
using HastaksharSewaAnalytics.Infrastructure.Persistence;
using System.Collections.Concurrent;

namespace HastaksharSewaAnalytics.Infrastructure.Repository.Common;

public sealed class UnitOfWork : IUnitOfWork
{
    private readonly HastaksharSewaAnalyticsDbContext _context;
    private readonly ConcurrentDictionary<(Type entityType, Type idType), object> _repositories = new();

    public UnitOfWork(HastaksharSewaAnalyticsDbContext context) => _context = context;

    public IGenericRepository<TEntity, TId> Repository<TEntity, TId>()
        where TEntity : Entity<TId>
        where TId : notnull
    {
        var key = (typeof(TEntity), typeof(TId));

        if (!_repositories.TryGetValue(key, out var repo))
        {
            repo = new GenericRepository<TEntity, TId>(_context);
            _repositories[key] = repo;
        }

        return (IGenericRepository<TEntity, TId>)repo;
    }

    public Task<int> SaveChangesAsync(CancellationToken ct = default)
        => _context.SaveChangesAsync(ct);

    public ValueTask DisposeAsync()
        => _context.DisposeAsync();
}
