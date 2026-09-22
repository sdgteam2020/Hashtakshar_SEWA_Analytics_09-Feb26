using HastaksharSewaAnalytics.Domain.Premitives.AuditEntity;

namespace HastaksharSewaAnalytics.Domain.Entities;

public sealed class VaultMaster : ClientAuditableEntity<int>
{
    private VaultMaster() : base(0) { }
    private VaultMaster(int id) : base(id) { }

    public string Public_Key { get; private set; } = string.Empty;
    public string SerialNo { get; private set; } = string.Empty;
    public bool TokenValid { get; private set; } = true;
    public DateTimeOffset? ValidFrom { get; private set; }
    public DateTimeOffset? ValidTo { get; private set; }

    public static VaultMaster Create(
        string publicKey,
        string serialNo,
        bool tokenValid,
        DateTimeOffset? validFrom,
        DateTimeOffset? validTo)
    {
        if (string.IsNullOrWhiteSpace(publicKey))
            throw new ArgumentException("Public key is required.", nameof(publicKey));
        if (string.IsNullOrWhiteSpace(serialNo))
            throw new ArgumentException("SerialNo is required.", nameof(serialNo));
        if (validFrom.HasValue && validTo.HasValue && validTo < validFrom)
            throw new ArgumentException("ValidTo cannot be earlier than ValidFrom.", nameof(validTo));

        var entity = new VaultMaster(0)
        {
            Public_Key = publicKey.Trim(),
            SerialNo = serialNo.Trim(),
            TokenValid = tokenValid,
            ValidFrom = validFrom,
            ValidTo = validTo
        };
        return entity;

    }

    public void UpdateCertificate(
        string publicKey,
        bool tokenValid,
        DateTimeOffset? validFrom,
        DateTimeOffset? validTo,
        string modifiedBy)
    {
        if (string.IsNullOrWhiteSpace(publicKey))
            throw new ArgumentException("Public key is required.", nameof(publicKey));
        if (validFrom.HasValue && validTo.HasValue && validTo < validFrom)
            throw new ArgumentException("ValidTo cannot be earlier than ValidFrom.", nameof(validTo));

        Public_Key = publicKey.Trim();
        TokenValid = tokenValid;
        ValidFrom = validFrom;
        ValidTo = validTo;
    }

    public void Activate(string modifiedBy)
    {
        if (TokenValid) return;
        TokenValid = true;
    }

    public void Deactivate(string modifiedBy)
    {
        if (!TokenValid) return;
        TokenValid = false;
    }
}
