using HastaksharSewaAnalytics.Domain.Premitives.AuditEntity;
using System.ComponentModel.DataAnnotations;
namespace HastaksharSewaAnalytics.Domain.Entities;

public sealed class VaultMaster : AuditableEntity<int>
{
    private VaultMaster(
        int id,      
        string publicKey,
        string serialNo,
        bool tokenValid,
        string validFrom,
        string validTo
    ) : base(id)
    {
        SetPublicKey(publicKey);
        SetSerialNo(serialNo);
        TokenValid = tokenValid;
        SetValidFrom(validFrom);
        SetValidTo(validTo);
    } 
    private VaultMaster() : base(0) { }

    [Required]
    public string Public_Key { get; private set; } = "";

    [Required]
    public string SerialNo { get; private set; } = "";

    [Required]
    public bool TokenValid { get; private set; }=true;

    [Required]
    public string ValidFrom { get; private set; } = DateOnly.MinValue.ToString();
    [Required]
    public string ValidTo { get; private set; } = DateOnly.MinValue.ToString();

     
    public static VaultMaster Create(        
        string publicKey,
        string serialNo,
        bool tokenValid,
        string validFrom,
        string validTo
    )
    {
        var entity = new VaultMaster(
            id: 0,
            publicKey,
            serialNo,
            tokenValid,
            validFrom,
            validTo
        );
         
        return entity;
    } 
    public void Update(
        string name,
        string rank,
        string publicKey,
        string serialNo,
        string tokenExpiryDate
    )
    {
        SetPublicKey(publicKey);
        SetSerialNo(serialNo);
        SetValidFrom(tokenExpiryDate);
    }

    public void Activate(string modifiedBy)
    {
        if (TokenValid) return;
        TokenValid = true;
        SetModified(modifiedBy);
    }

    public void Deactivate(string modifiedBy)
    {
        if (!TokenValid) return;
        TokenValid = false;
        SetModified(modifiedBy);
    }

    private void SetPublicKey(string publicKey)
    {
        if (string.IsNullOrWhiteSpace(publicKey))
            throw new ArgumentException("Public key is required.", nameof(publicKey));

        Public_Key = publicKey.Trim();
    }

    private void SetSerialNo(string serialNo)
    {
        if (string.IsNullOrWhiteSpace(serialNo))
            throw new ArgumentException("SerialNo is required.", nameof(serialNo));

        SerialNo = serialNo.Trim();
    }

    private void SetValidFrom(string validFrom)
    {
        if (string.IsNullOrWhiteSpace(validFrom))
            throw new ArgumentException("ValidFrom Date is required.", nameof(validFrom));
        ValidFrom = validFrom.Trim();
    }

    private void SetValidTo(string validTo)
    {
        if (string.IsNullOrWhiteSpace(validTo))
            throw new ArgumentException("ValidTo Date is required.", nameof(validTo));
        ValidTo = validTo.Trim();
    }
}
