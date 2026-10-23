using System.ComponentModel.DataAnnotations;
namespace HastaksharSewaAnalytics.Application.Dtos.Transaction;

public record SaveDailyRunRequest(

    // Domain ID
    [param: Required(ErrorMessage = "Domain ID is required.")]
    [param: StringLength(100, ErrorMessage = "Domain ID cannot exceed 100 characters.")]
    [param: RegularExpression( @"^[A-Za-z0-9][A-Za-z0-9._-]*$", ErrorMessage = "Invalid Domain ID.")]
    string DomainId,

    // IP Address (IPv4)
    [param: Required(ErrorMessage = "IP Address is required.")]
    [param: RegularExpression(@"^(?:(?:25[0-5]|2[0-4]\d|1\d{2}|[1-9]?\d)\.){3}(?:25[0-5]|2[0-4]\d|1\d{2}|[1-9]?\d)$", ErrorMessage = "Invalid IPv4 Address.")]
    string IpAddress,

    // Application Version (Example: 2.0.0.0)
    [param: Required(ErrorMessage = "Version is required.")]
    [param: RegularExpression(@"^\d+\.\d+\.\d+\.\d+$", ErrorMessage = "Invalid Version. Expected format: 2.0.0.0")]
    string Version,

    // Created By (Optional)
    [param: StringLength(100, ErrorMessage = "Created By cannot exceed 100 characters.")]
    [param: RegularExpression(@"^[A-Za-z0-9][A-Za-z0-9._@-]*$", ErrorMessage = "Invalid Created By.")]
    string? CreatedBy = null,

    // Device ID (Optional)
    [param: StringLength(100, ErrorMessage = "Device ID cannot exceed 100 characters.")]
    [param: RegularExpression( @"^[A-Za-z0-9][A-Za-z0-9_-]*$", ErrorMessage = "Invalid Device ID.")]
    string? DeviceId = null
);
