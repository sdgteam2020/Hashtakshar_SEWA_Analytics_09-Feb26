namespace HastaksharSewaAnalytics.Application.Dtos.Transaction;

public record SaveDailyRunRequest(
    string domainId,
    string ipAddress,
    string version,
    DateTimeOffset? runOnDate = null,
    string? createdBy = null
    );
