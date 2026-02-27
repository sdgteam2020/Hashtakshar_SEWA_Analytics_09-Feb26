namespace HastaksharSewaAnalytics.Application.Dtos.Dashboard;

public sealed record GetPublicKeyValtResponse(
    int Id,
    string Public_Key,
    string SerialNo,
    bool TokenValid,
    string ValidFrom,
    string ValidTo,
    string CreatedBy,
    string CreatedAt
    );