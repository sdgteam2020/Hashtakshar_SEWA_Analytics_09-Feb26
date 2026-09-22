namespace HastaksharSewaAnalytics.Application.Dtos.Transaction;

public record SaveDailyRunRequest(
    string DomainId,
    string IpAddress,
    string Version,
    string? CreatedBy = null,
    string? DeviceId = null
);
