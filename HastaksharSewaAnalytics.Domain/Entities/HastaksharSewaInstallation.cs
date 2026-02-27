using HastaksharSewaAnalytics.Domain.Premitives.AuditEntity;

namespace HastaksharSewaAnalytics.Domain.Entities;

public sealed class HastaksharSewaInstallation : AuditableEntity<int>
{
    private HastaksharSewaInstallation() : base(0) { }

    private HastaksharSewaInstallation(
        int id,
        string domainId,
        string ipAddress,
        string version,
        DateTimeOffset installDate
    ) : base(id)
    {
        SetDomainId(domainId);
        SetIpAddress(ipAddress);
        SetVersion(version);
        InstallDate = installDate;
    }

    public string DomainId { get; private set; } = "";
    public string IPAddress { get; private set; } = "";
    public string Version { get; private set; } = "";
    public DateTimeOffset InstallDate { get; private set; }


    public static HastaksharSewaInstallation Create(
        string domainId,
        string ipAddress,
        string version,
        DateTimeOffset? installDate = null,
        int? id = null
    )
    {
        var entity = new HastaksharSewaInstallation(
            id: 0,
            domainId,
            ipAddress,
            version,
            installDate ?? DateTimeOffset.UtcNow
        );

        return entity;
    }

    public void UpdateInstallation(string ipAddress, string version, string modifiedBy)
    {
        SetIpAddress(ipAddress);
        SetVersion(version);
        SetModified(modifiedBy);
    }

    public void ChangeDomain(string domainId, string modifiedBy)
    {
        SetDomainId(domainId);
        SetModified(modifiedBy);
    }

    private void SetDomainId(string domainId)
    {
        if (string.IsNullOrWhiteSpace(domainId))
            throw new ArgumentException("DomainId is required.", nameof(domainId));

        if (domainId.Length > 64)
            throw new ArgumentException("DomainId max length is 64.", nameof(domainId));

        DomainId = domainId.Trim();
    }

    private void SetIpAddress(string ipAddress)
    {
        if (string.IsNullOrWhiteSpace(ipAddress))
            throw new ArgumentException("IPAddress is required.", nameof(ipAddress));

        IPAddress = ipAddress.Trim();
    }

    private void SetVersion(string version)
    {
        if (string.IsNullOrWhiteSpace(version))
            throw new ArgumentException("Version is required.", nameof(version));

        if (version.Length > 32)
            throw new ArgumentException("Version max length is 32.", nameof(version));

        Version = version.Trim();
    }
}
