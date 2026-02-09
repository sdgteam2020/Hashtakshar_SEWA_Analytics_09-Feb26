using HastaksharSewaAnalytics.Domain.Premitives.Entity;

namespace HastaksharSewaAnalytics.Domain.Premitives.AuditEntity;

public abstract class AuditableEntity<TId> : Entity<TId>
    where TId : notnull
{
    protected AuditableEntity(TId id) : base(id) { }

    public string CreatedBy { get; private set; } = "System";
    public DateTimeOffset CreatedAt { get; private set; } = DateTime.UtcNow;

    public string ModifiedBy { get; private set; } = "";
    public DateTimeOffset? ModifiedOn { get; private set; }

    public void SetCreated(string createdBy)
    {
        CreatedBy = createdBy;
        CreatedAt = DateTimeOffset.UtcNow;
    }

    public void SetModified(string modifiedBy)
    {
        ModifiedBy = modifiedBy;
        ModifiedOn = DateTimeOffset.UtcNow;
    }
}

