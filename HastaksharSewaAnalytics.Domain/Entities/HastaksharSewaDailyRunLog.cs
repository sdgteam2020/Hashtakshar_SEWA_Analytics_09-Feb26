using HastaksharSewaAnalytics.Domain.Premitives.AuditEntity;
using System.ComponentModel.DataAnnotations;

namespace HastaksharSewaAnalytics.Domain.Entities;

public sealed class HastaksharSewaDailyRunLog : AuditableEntity<Guid>
{
    // EF Core needs parameterless ctor (can be private)
    private HastaksharSewaDailyRunLog() : base(Guid.Empty) { }

    private HastaksharSewaDailyRunLog(
        Guid id,
        string domainId,
        string ipAddress,
        string version,
        DateTimeOffset runOnDate
    ) : base(id)
    {
        SetDomainId(domainId);
        SetIpAddress(ipAddress);
        SetVersion(version);
        RunOnDate = runOnDate;
    }

    [Required, MaxLength(64)]
    public string DomainId { get; private set; } = "";

    [Required]
    public string IPAddress { get; private set; } = "";

    [Required, MaxLength(32)]
    public string Version { get; private set; } = "";

    [Required]
    public DateTimeOffset RunOnDate { get; private set; }

    // ✅ Factory (Create)
    public static HastaksharSewaDailyRunLog Create(
        string domainId,
        string ipAddress,
        string version,
        DateTimeOffset? runOnDate = null,
        string? createdBy = null
    )
    {
        var entity = new HastaksharSewaDailyRunLog(
            Guid.NewGuid(),
            domainId,
            ipAddress,
            version,
            runOnDate ?? DateTime.UtcNow
        );

        if (!string.IsNullOrWhiteSpace(createdBy))
            entity.SetCreated(createdBy);

        return entity;
    }

    // ✅ Rich behaviour (keep entity valid)
    public void UpdateVersion(string version, string modifiedBy)
    {
        SetVersion(version);
        SetModified(modifiedBy);
    }

    public void UpdateIp(string ipAddress, string modifiedBy)
    {
        SetIpAddress(ipAddress);
        SetModified(modifiedBy);
    }

    public void UpdateDomain(string domainId, string modifiedBy)
    {
        SetDomainId(domainId);
        SetModified(modifiedBy);
    }

    public void MarkRunDate(DateTimeOffset runOnDate, string modifiedBy)
    {
        RunOnDate = runOnDate;
        SetModified(modifiedBy);
    }

    // Optional: convenience for daily jobs
    public void MarkRunTodayUtc(string modifiedBy)
        => MarkRunDate(DateTimeOffset.UtcNow, modifiedBy);

    // -----------------------
    // Guards / invariants
    // -----------------------
    private void SetDomainId(string domainId)
    {
        if (string.IsNullOrWhiteSpace(domainId))
            throw new ArgumentException("DomainId is required.", nameof(domainId));

        domainId = domainId.Trim();
        if (domainId.Length > 64)
            throw new ArgumentException("DomainId max length is 64.", nameof(domainId));

        DomainId = domainId;
    }

    private void SetVersion(string version)
    {
        if (string.IsNullOrWhiteSpace(version))
            throw new ArgumentException("Version is required.", nameof(version));

        version = version.Trim();
        if (version.Length > 32)
            throw new ArgumentException("Version max length is 32.", nameof(version));

        Version = version;
    }

    private void SetIpAddress(string ipAddress)
    {
        if (string.IsNullOrWhiteSpace(ipAddress))
            throw new ArgumentException("IPAddress is required.", nameof(ipAddress));

        ipAddress = ipAddress.Trim();
        if (!System.Net.IPAddress.TryParse(ipAddress, out _))
            throw new ArgumentException("Invalid IP address format.", nameof(ipAddress));

        IPAddress = ipAddress;
    }
}
