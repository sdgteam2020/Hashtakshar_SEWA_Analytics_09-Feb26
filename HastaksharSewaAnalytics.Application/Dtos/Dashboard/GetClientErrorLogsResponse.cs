namespace HastaksharSewaAnalytics.Application.Dtos.Dashboard;

public sealed record GetClientErrorLogsResponse(
    Guid Id,
    string AppName,
    string ErrorMessage,
    string StackTrace,
    string IpAddress,
    string MachineName,
    string UserName,
    string OperatingSystem,
    bool Is64Bit,
    string SystemDirectory,
    string AppVersion,
    string Extra,
    string LoggedAt
    );
