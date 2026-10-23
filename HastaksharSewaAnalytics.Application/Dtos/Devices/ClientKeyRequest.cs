using System.ComponentModel.DataAnnotations;
namespace HastaksharSewaAnalytics.Application.Dtos.Devices;


public sealed record ClientKeyRequest
{
    // Domain ID
    [Required(ErrorMessage = "Domain ID is required.")]
    [StringLength(100, ErrorMessage = "Domain ID cannot exceed 100 characters.")]
    [RegularExpression(        @"^[A-Za-z0-9][A-Za-z0-9._-]*$", ErrorMessage = "Invalid Domain ID. Only letters, numbers, dots, underscores and hyphens are allowed.")]
    public string DomainId { get; set; } = string.Empty;

    // IP Address (IPv4)
    [Required(ErrorMessage = "IP Address is required.")]
    [RegularExpression(@"^(?:(?:25[0-5]|2[0-4]\d|1\d{2}|[1-9]?\d)\.){3}(?:25[0-5]|2[0-4]\d|1\d{2}|[1-9]?\d)$", ErrorMessage = "Invalid IPv4 address.")]
    public string IPAddress { get; set; } = string.Empty;

    // Client Key (GUID)
    [Required(ErrorMessage = "Client Key is required.")]
    [RegularExpression(@"^[a-fA-F0-9]{32}$", ErrorMessage = "Invalid Client Key. Expected a 32-character GUID.")]
    public string ClientKey { get; set; } = string.Empty;
}