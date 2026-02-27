namespace HastaksharSewaAnalytics.Application.Dtos.Dashboard;

public record GetInstallAppDataResponse(
    int Id,
    string DomainId,
    string IPAddress,
    string Version,
    string InstallDate);

