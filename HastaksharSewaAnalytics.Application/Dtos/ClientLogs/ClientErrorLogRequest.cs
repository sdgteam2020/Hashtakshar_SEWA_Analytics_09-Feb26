namespace HastaksharSewaAnalytics.Application.Dtos.ClientLogs;

public record ClientErrorLogRequest(
    string IpAddress,
    string MachineName,
    string? UserName,
    string OperatingSystem,
    bool Is64Bit,
    string SystemDirectory,
    string AppName,
    string? AppVersion,
    string ErrorMessage,
    string? StackTrace,
    string? Extra
    );
