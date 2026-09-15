using HastaksharSewaAnalytics.Domain.Premitives.AuditEntity;

namespace HastaksharSewaAnalytics.Domain.Entities;

public sealed class HastaksharSewaDailyRunLog : AuditableEntity<int>
{
    private HastaksharSewaDailyRunLog() : base(0) { }
    private HastaksharSewaDailyRunLog(int id) : base(id) { }

    public int ClientId { get; private set; }
    public int VersionId { get; private set; }
    public DateOnly RunOnDate { get; private set; }

    public static HastaksharSewaDailyRunLog Create(
        int clientId,
        int versionId,
        DateOnly? runOnDate = null,
        string? createdBy = null)
    {
        if (clientId <= 0)
            throw new ArgumentOutOfRangeException(nameof(clientId));
        if (versionId <= 0)
            throw new ArgumentOutOfRangeException(nameof(versionId));

        var entity = new HastaksharSewaDailyRunLog(0)
        {
            ClientId = clientId,
            VersionId = versionId,
            RunOnDate = runOnDate ?? DateOnly.FromDateTime(DateTime.UtcNow)
        };

        if (!string.IsNullOrWhiteSpace(createdBy))
            entity.SetCreated(createdBy);

        return entity;
    }
}
