using HastaksharSewaAnalytics.Domain.Premitives.AuditEntity;

namespace HastaksharSewaAnalytics.Domain.Entities;

public sealed class ClientMaster : AuditableEntity<int>
{
    private ClientMaster() : base(0) { }
    private ClientMaster(int id) : base(id) { }

    public string DomainId { get; private set; } = string.Empty;
    public string IPAddress { get; private set; } = string.Empty;
    public int? DeviceId { get; private set; }

    public static ClientMaster Create(
        string domainId,
        string ipAddress,
        int? deviceId = null,
        string? createdBy = null)
    {
        var entity = new ClientMaster(0);
        entity.SetDomainId(domainId);
        entity.SetIpAddress(ipAddress);
        entity.DeviceId = deviceId;

        if (!string.IsNullOrWhiteSpace(createdBy))
            entity.SetCreated(createdBy);

        return entity;
    }

    public void UpdateConnection(string ipAddress, int? deviceId, string modifiedBy)
    {
        SetIpAddress(ipAddress);
        if (deviceId.HasValue)
            DeviceId = deviceId;
        SetModified(modifiedBy);
    }

    private void SetDomainId(string domainId)
    {
        if (string.IsNullOrWhiteSpace(domainId))
            throw new ArgumentException("DomainId is required.", nameof(domainId));

        domainId = domainId.Trim();
        if (domainId.Length > 64)
            throw new ArgumentException("DomainId max length is 64.", nameof(domainId));

        DomainId = domainId;
    }

    private void SetIpAddress(string ipAddress)
    {
        if (string.IsNullOrWhiteSpace(ipAddress))
            throw new ArgumentException("IPAddress is required.", nameof(ipAddress));

        ipAddress = ipAddress.Trim();
        if (ipAddress.Length > 64)
            throw new ArgumentException("IPAddress max length is 64.", nameof(ipAddress));
        if (!System.Net.IPAddress.TryParse(ipAddress, out _))
            throw new ArgumentException("Invalid IP address format.", nameof(ipAddress));

        IPAddress = ipAddress;
    }
}
