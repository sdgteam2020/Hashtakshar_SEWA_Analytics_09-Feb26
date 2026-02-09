namespace HastaksharSewaAnalytics.Application.Dtos.DigitalSign;

public sealed record GetDigitalSignRespone(
    Guid Id,
    Guid UserPublicDataId,
    string DocumentName,
    string SignedAt,
    string IpAddress
    );