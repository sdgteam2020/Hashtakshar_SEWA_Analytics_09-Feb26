namespace HastaksharSewaAnalytics.Application.Dtos.ClientLogs;

public record ClientErrorLogRequest(
    string IpAddress, 
    string MachineName,  
    string ErrorMessage,
    string AppName,
    string? AppVersion
);
