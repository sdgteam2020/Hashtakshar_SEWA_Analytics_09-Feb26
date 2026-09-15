using HastaksharSewaAnalytics.Domain.Premitives.AuditEntity;

namespace HastaksharSewaAnalytics.Domain.Entities;

public sealed class HastaksharSewaInstallation : AuditableEntity<int>
{
    private HastaksharSewaInstallation() : base(0) { }
    private HastaksharSewaInstallation(int id) : base(id) { }

    public int ClientId { get; private set; }
    public int VersionId { get; private set; }
    public DateTimeOffset InstallDate { get; private set; }

    public static HastaksharSewaInstallation Create(
        int clientId,
        int versionId,
        DateTimeOffset? installDate = null,
        string? createdBy = null)
    {
        if (clientId <= 0)
            throw new ArgumentOutOfRangeException(nameof(clientId));
        if (versionId <= 0)
            throw new ArgumentOutOfRangeException(nameof(versionId));

        var entity = new HastaksharSewaInstallation(0)
        {
            ClientId = clientId,
            VersionId = versionId,
            InstallDate = installDate ?? DateTimeOffset.UtcNow
        };

        if (!string.IsNullOrWhiteSpace(createdBy))
            entity.SetCreated(createdBy);

        return entity;
    }
}
