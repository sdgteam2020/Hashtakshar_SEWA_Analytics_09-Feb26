using HastaksharSewaAnalytics.Domain.Premitives.AuditEntity;

namespace HastaksharSewaAnalytics.Domain.Entities;

public sealed class HastaksharSewaInstallation : ClientAuditableEntity<int>
{
    private HastaksharSewaInstallation() : base(0) { }
    private HastaksharSewaInstallation(int id) : base(id) { }

    public int VersionId { get; private set; }

    public static HastaksharSewaInstallation Create(
        int clientId,
        int versionId)
    {
        if (clientId <= 0)
            throw new ArgumentOutOfRangeException(nameof(clientId));
        if (versionId <= 0)
            throw new ArgumentOutOfRangeException(nameof(versionId));

        var entity = new HastaksharSewaInstallation(0);
        entity.SetCreated(clientId);
        entity.VersionId = versionId;
       
        return entity;
    }
}
