using HastaksharSewaAnalytics.Domain.Premitives.Entity;

namespace HastaksharSewaAnalytics.Domain.Premitives.AuditEntity;

public abstract class ClientRootAuditableEntity<TId> : Entity<TId>
    where TId : notnull
{
    protected ClientRootAuditableEntity(TId id) : base(id) { }

    public DateTimeOffset CreatedAt { get; private set; } = DateTime.UtcNow;

    protected void SetCreatedAt()
    {
        CreatedAt = DateTimeOffset.UtcNow;
    }
}
