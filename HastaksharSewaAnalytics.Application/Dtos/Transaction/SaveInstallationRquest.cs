namespace HastaksharSewaAnalytics.Application.Dtos.Transaction;

public record SaveInstallationRquest(
    string DomainId,
    string IpAddress,
    string Version,
    DateTimeOffset? InstallDate = null,
    Guid? Id = null
    );