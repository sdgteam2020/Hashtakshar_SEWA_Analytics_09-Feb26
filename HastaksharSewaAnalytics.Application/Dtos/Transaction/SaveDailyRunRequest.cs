namespace HastaksharSewaAnalytics.Application.Dtos.Transaction;

public record SaveDailyRunRequest(
    string DomainId,
    string IpAddress,
    string Version,
    DateTimeOffset? RunOnDate = null,
    string? CreatedBy = null
    );
