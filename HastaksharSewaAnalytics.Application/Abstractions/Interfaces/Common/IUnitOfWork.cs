using HastaksharSewaAnalytics.Domain.Premitives.Entity;

namespace HastaksharSewaAnalytics.Application.Abstractions.Interfaces.Common;

public interface IUnitOfWork : IAsyncDisposable
{
    IGenericRepository<TEntity, TId> Repository<TEntity, TId>()
        where TEntity : Entity<TId>
        where TId : notnull;

    Task<int> SaveChangesAsync(CancellationToken ct = default);    

}

