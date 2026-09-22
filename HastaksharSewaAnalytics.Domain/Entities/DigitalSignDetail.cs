using HastaksharSewaAnalytics.Domain.Premitives.AuditEntity;

namespace HastaksharSewaAnalytics.Domain.Entities;

public sealed class DigitalSignDetail : ClientAuditableEntity<int>
{
    private DigitalSignDetail() : base(0) { }
    private DigitalSignDetail(int id) : base(id) { }

    public int VaultMasterId { get; private set; }    
    public DateTimeOffset? SignDateTime { get; private set; }   
    public string? DocumentName { get; private set; }
  
    public static DigitalSignDetail Create(
        int vaultMasterId,
        int clientId,
        DateTimeOffset? signDateTime,        
        string? documentName)
    {
        if (vaultMasterId <= 0)
            throw new ArgumentOutOfRangeException(nameof(vaultMasterId));

        if (clientId <= 0)
            throw new ArgumentOutOfRangeException(nameof(clientId));

        if (signDateTime == null)
            throw new ArgumentNullException(nameof(signDateTime));
        var entity = new DigitalSignDetail(0);
        entity.SetCreated(clientId);
        entity.VaultMasterId = vaultMasterId;
        entity.SignDateTime = signDateTime;
        entity.DocumentName = Normalize(documentName, 512);
        return entity;       
    }

    private static string? Normalize(string? value, int maxLen)
    {
        if (string.IsNullOrWhiteSpace(value)) return null;
        value = value.Trim();
        return value.Length <= maxLen ? value : value[..maxLen];
    }
}
