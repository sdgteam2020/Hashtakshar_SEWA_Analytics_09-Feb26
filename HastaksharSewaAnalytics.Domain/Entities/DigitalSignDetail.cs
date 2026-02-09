using HastaksharSewaAnalytics.Domain.Premitives.AuditEntity;

namespace HastaksharSewaAnalytics.Domain.Entities;

public sealed class DigitalSignDetail: AuditableEntity<Guid>
{
    private DigitalSignDetail(): base(Guid.Empty) { }

    public Guid ValtMasterId { get; private set; }
    public string? SignDateTime { get; private set; }
    public string? OriginForSign { get; private set; }
    public string? RefererForSign { get; private set; }
    public string? IpAddress { get; private set; }
    public string? DocumentName { get; private set; }
    public string? DocumnetType { get; private set; }

    public DigitalSignDetail(Guid id): base(Guid.NewGuid()) { }

    public static DigitalSignDetail Create(
        Guid publicUserDataID,
        string signDateTime,
        string? originForSign,
        string? refererForSign,
        string? ipAddress,
        string? documentName,
        string? documnetType
    )
    {
        var entity = new DigitalSignDetail()
        {
            ValtMasterId = publicUserDataID,
            SignDateTime = signDateTime,
            OriginForSign = originForSign,
            RefererForSign = refererForSign,
            IpAddress = ipAddress,
            DocumentName = documentName,
            DocumnetType = documnetType
        };
        return entity;
    }
}
