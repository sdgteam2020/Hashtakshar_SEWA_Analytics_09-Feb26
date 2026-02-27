namespace HastaksharSewaAnalytics.Application.Dtos.DigitalSign;

public sealed record GetDigitalSignRespone(
    int Id,
    int UserPublicDataId,
    string DocumentName,
    string SignedAt,
    string IpAddress
    );