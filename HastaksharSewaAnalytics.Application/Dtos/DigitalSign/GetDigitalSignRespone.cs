namespace HastaksharSewaAnalytics.Application.Dtos.DigitalSign;

public sealed record GetDigitalSignRespone(
    int Id,
    int UserPublicDataId,
    string IPAddress,
    string DocumentName,
    string SignedAt    
    );