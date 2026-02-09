namespace HastaksharSewaAnalytics.Application.Dtos.DigitalSign;

public sealed record SaveDigitalSignRequest(   
    string PublicKey,
    string SerialNo,
    bool TokenValid,
    string ValidFrom,
    string ValidTo,
    string SignedDateTime,
    string? OriginForSign,
    string? RefererForSign,
    string? IpAddress,
    string? DocumentName,
    string? DocumnetType
    );
