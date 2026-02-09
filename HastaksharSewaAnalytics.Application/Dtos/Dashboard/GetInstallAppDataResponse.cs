namespace HastaksharSewaAnalytics.Application.Dtos.Dashboard;

public record GetInstallAppDataResponse(
    Guid Id,
    string DomainId,
    string IPAddress,
    string Version,
    string InstallDate);

