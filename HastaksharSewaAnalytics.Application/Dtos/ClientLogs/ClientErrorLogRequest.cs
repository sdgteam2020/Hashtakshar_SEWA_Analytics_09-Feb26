using System.ComponentModel.DataAnnotations;
namespace HastaksharSewaAnalytics.Application.Dtos.ClientLogs;


using System.ComponentModel.DataAnnotations;

public record ClientErrorLogRequest(

    // IPv4 Address
    [param: Required(ErrorMessage = "IP Address is required.")]
    [param: RegularExpression(
        @"^(?:(?:25[0-5]|2[0-4]\d|1\d{2}|[1-9]?\d)\.){3}(?:25[0-5]|2[0-4]\d|1\d{2}|[1-9]?\d)$",
        ErrorMessage = "Invalid IPv4 Address.")]
    string IpAddress,

    // Machine Name
    [param: Required(ErrorMessage = "Machine Name is required.")]
    [param: RegularExpression(
        @"^[A-Za-z0-9](?:[A-Za-z0-9_-]{0,61}[A-Za-z0-9])?$",
        ErrorMessage = "Invalid Machine Name.")]
    string MachineName,

    // Error Message
    [param: Required(ErrorMessage = "Error Message is required.")]
    [param: StringLength(10000,
        ErrorMessage = "Error Message cannot exceed 10000 characters.")]
    string ErrorMessage,

    // Application Name
    [param: Required(ErrorMessage = "App Name is required.")]
    [param: RegularExpression(
        @"^[A-Za-z0-9][A-Za-z0-9 ._-]{0,99}$",
        ErrorMessage = "Invalid App Name.")]
    string AppName,

    // Application Version - Optional
    [param: RegularExpression(
        @"^\d+\.\d+\.\d+\.\d+$",
        ErrorMessage = "Invalid App Version. Expected format: 2.0.0.0")]
    string? AppVersion
);