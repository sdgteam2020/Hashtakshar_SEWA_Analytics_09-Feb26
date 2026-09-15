using HastaksharSewaAnalytics.Domain.Premitives.AuditEntity;

namespace HastaksharSewaAnalytics.Domain.Entities;

public sealed class ApplicationVersion : AuditableEntity<int>
{
    private ApplicationVersion() : base(0) { }
    private ApplicationVersion(int id) : base(id) { }

    public int ApplicationId { get; private set; }
    public string Version { get; private set; } = string.Empty;

    public static ApplicationVersion Create(int applicationId, string version, string? createdBy = null)
    {
        if (applicationId <= 0)
            throw new ArgumentOutOfRangeException(nameof(applicationId));
        if (string.IsNullOrWhiteSpace(version))
            throw new ArgumentException("Version is required.", nameof(version));

        version = version.Trim();
        if (version.Length > 50)
            throw new ArgumentException("Version max length is 50.", nameof(version));

        var entity = new ApplicationVersion(0)
        {
            ApplicationId = applicationId,
            Version = version
        };

        if (!string.IsNullOrWhiteSpace(createdBy))
            entity.SetCreated(createdBy);

        return entity;
    }
}
