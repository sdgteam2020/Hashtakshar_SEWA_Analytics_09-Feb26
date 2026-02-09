namespace HastaksharSewaAnalytics.Application.Dtos.Dashboard;

public sealed record GetPublicKeyValtResponse(
    Guid Id,
    string Public_Key,
    string SerialNo,
    bool TokenValid,
    string ValidFrom,
    string ValidTo,
    string CreatedBy,
    string CreatedAt
    );