using HastaksharSewaAnalytics.Domain.Premitives.AuditEntity;

namespace HastaksharSewaAnalytics.Domain.Entities;

public sealed class ClientKey : ClientAuditableEntity<int>
{
    private ClientKey() : base(0)
    {
    }

    public string ClientKeyHash { get; set; } = default!;     // hashed
    public string ClientKeySalt { get; set; } = default!;     // base64 salt
    public bool IsActive { get; set; } = true;

    private ClientKey(int id) : base(id) { }

    public static ClientKey Create(
        int clientId,
        string clientKeyHash,
        string clientKeySalt)
    {
        if (clientId == 0)
            throw new ArgumentException("ClientId is required.", nameof(clientId));
        if (string.IsNullOrWhiteSpace(clientKeyHash))
            throw new ArgumentException("ClientKeyHash is required.", nameof(clientKeyHash));
        if (string.IsNullOrWhiteSpace(clientKeySalt))
            throw new ArgumentException("ClientKeySalt is required.", nameof(clientKeySalt));

        var entity = new ClientKey(0);
        entity.SetCreated(clientId);
        entity.ClientKeyHash = clientKeyHash.Trim();
        entity.ClientKeySalt = clientKeySalt.Trim();
        entity.IsActive = true;
        
        return entity;
    }
}