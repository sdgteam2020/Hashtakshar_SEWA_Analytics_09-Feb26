using HastaksharSewaAnalytics.Domain.Premitives.AuditEntity;

namespace HastaksharSewaAnalytics.Domain.Entities;

public sealed class DigitalSignDetail : AuditableEntity<int>
{
    private DigitalSignDetail() : base(0) { }
    private DigitalSignDetail(int id) : base(id) { }

    public int VaultMasterId { get; private set; }
    public DateTimeOffset? SignDateTime { get; private set; }
    public string? OriginForSign { get; private set; }
    public string? RefererForSign { get; private set; }
    public string? IpAddress { get; private set; }
    public string? DocumentName { get; private set; }
    public string? DocumentHash { get; private set; }
    public string? DocumnetType { get; private set; }

    public static DigitalSignDetail Create(
        int vaultMasterId,
        DateTimeOffset? signDateTime,
        string? originForSign,
        string? refererForSign,
        string? ipAddress,
        string? documentName,
        string? documnetType,
        string? documentHash = null)
    {
        if (vaultMasterId <= 0)
            throw new ArgumentOutOfRangeException(nameof(vaultMasterId));

        return new DigitalSignDetail(0)
        {
            VaultMasterId = vaultMasterId,
            SignDateTime = signDateTime,
            OriginForSign = Normalize(originForSign, 2048),
            RefererForSign = Normalize(refererForSign, 2048),
            IpAddress = Normalize(ipAddress, 64),
            DocumentName = Normalize(documentName, 512),
            DocumentHash = Normalize(documentHash, 256),
            DocumnetType = Normalize(documnetType, 100)
        };
    }

    private static string? Normalize(string? value, int maxLen)
    {
        if (string.IsNullOrWhiteSpace(value)) return null;
        value = value.Trim();
        return value.Length <= maxLen ? value : value[..maxLen];
    }
}
