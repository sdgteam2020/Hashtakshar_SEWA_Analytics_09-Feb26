namespace HastaksharSewaAnalytics.Application.Dtos.Dashboard;

public sealed record GetClientErrorLogsResponse(
    int Id,
    string AppName,
    string ErrorMessage,
    string IpAddress,
    string MachineName,
    string AppVersion,
    string LoggedAt
    );
