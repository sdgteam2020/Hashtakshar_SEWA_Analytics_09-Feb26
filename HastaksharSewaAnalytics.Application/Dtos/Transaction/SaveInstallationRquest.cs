namespace HastaksharSewaAnalytics.Application.Dtos.Transaction;

public record SaveInstallationRquest(
    string domainId,
    string ipAddress,
    string version,
    DateTimeOffset? installDate = null,
    Guid? id = null
    );