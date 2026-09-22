using HastaksharSewaAnalytics.Domain.Premitives.Entity;

namespace HastaksharSewaAnalytics.Domain.Premitives.AuditEntity;

public abstract class ClientAuditableEntity<TId> : Entity<TId>
    where TId : notnull
{
    protected ClientAuditableEntity(TId id) : base(id) { }

    public int CreatedByClientId { get; private set; } 
    public DateTimeOffset CreatedAt { get; private set; } = DateTime.UtcNow;

    public void SetCreated(int createdBy)
    {
        CreatedByClientId = createdBy;
        CreatedAt = DateTimeOffset.UtcNow;
    }
}

