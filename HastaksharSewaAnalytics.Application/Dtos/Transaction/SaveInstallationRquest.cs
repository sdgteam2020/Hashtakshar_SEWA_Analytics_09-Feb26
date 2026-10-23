using System.ComponentModel.DataAnnotations;
namespace HastaksharSewaAnalytics.Application.Dtos.Transaction;


public record SaveInstallationRquest(

    // Domain ID
    [property: Required(ErrorMessage = "Domain ID is required.")]
    [property: StringLength(100,
        ErrorMessage = "Domain ID cannot exceed 100 characters.")]
    [property: RegularExpression(@"^[A-Za-z0-9][A-Za-z0-9._-]*$", ErrorMessage = "Invalid Domain ID.")]
    string DomainId,

    // IP Address (IPv4)
    [property: Required(ErrorMessage = "IP Address is required.")]
    [property: RegularExpression(@"^(?:(?:25[0-5]|2[0-4]\d|1\d{2}|[1-9]?\d)\.){3}(?:25[0-5]|2[0-4]\d|1\d{2}|[1-9]?\d)$", ErrorMessage = "Invalid IPv4 Address.")]
    string IpAddress,

    // Application Version (Example: 2.0.0.0)
    [property: Required(ErrorMessage = "Version is required.")]
    [property: RegularExpression(@"^\d+\.\d+\.\d+\.\d+$", ErrorMessage = "Invalid Version. Expected format: 2.0.0.0")]
    string Version,

    // Installation Date (Optional)
    // DateTimeOffset is validated by model binding.
    DateTimeOffset? InstallDate = null,

    // Installation ID (Optional GUID)
    // No regex required for Guid.
    Guid? Id = null,

    // Device ID (Optional)
    [property: StringLength(100, ErrorMessage = "Device ID cannot exceed 100 characters.")]
    [property: RegularExpression(@"^[A-Za-z0-9][A-Za-z0-9_-]*$", ErrorMessage = "Invalid Device ID.")]
    string? DeviceId = null
);