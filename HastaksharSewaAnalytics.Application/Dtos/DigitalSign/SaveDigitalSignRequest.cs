using System.ComponentModel.DataAnnotations;
namespace HastaksharSewaAnalytics.Application.Dtos.DigitalSign;


public sealed record SaveDigitalSignRequest(

    // Public Key
    [param: Required(ErrorMessage = "Public Key is required.")]
    [param: StringLength(32768,
        ErrorMessage = "Public Key exceeds the maximum allowed length.")]
    string PublicKey,

    // Certificate Serial Number (Hexadecimal)
    [param: Required(ErrorMessage = "Serial Number is required.")]
    [param: RegularExpression(
        @"^[A-Fa-f0-9]{1,64}$",
        ErrorMessage = "Invalid Serial Number format.")]
    string SerialNo,

    // Token Validity (Boolean - No Regex Required)
    bool TokenValid,

    // Certificate Valid From
    [param: Required(ErrorMessage = "Valid From is required.")]
    [param: RegularExpression(
        @"^\d{4}-\d{2}-\d{2}T\d{2}:\d{2}:\d{2}(?:\.\d{1,7})?(?:Z|[+-]\d{2}:\d{2})?$",
        ErrorMessage = "Invalid Valid From date format.")]
    string ValidFrom,

    // Certificate Valid To
    [param: Required(ErrorMessage = "Valid To is required.")]
    [param: RegularExpression(
        @"^\d{4}-\d{2}-\d{2}T\d{2}:\d{2}:\d{2}(?:\.\d{1,7})?(?:Z|[+-]\d{2}:\d{2})?$",
        ErrorMessage = "Invalid Valid To date format.")]
    string ValidTo,

    // Signed Date Time
    [param: Required(ErrorMessage = "Signed Date Time is required.")]
    [param: RegularExpression(
        @"^\d{4}-\d{2}-\d{2}T\d{2}:\d{2}:\d{2}(?:\.\d{1,7})?(?:Z|[+-]\d{2}:\d{2})?$",
        ErrorMessage = "Invalid Signed Date Time format.")]
    string SignedDateTime,

    // Document Name (Optional)
    [param: RegularExpression(@"^[^<>:""/\\|?*\x00-\x1F]{1,255}$", ErrorMessage = "Invalid Document Name.")]
    string? DocumentName,

    // IP Address (Optional - IPv4)
    [param: RegularExpression(@"^(?:(?:25[0-5]|2[0-4]\d|1\d{2}|[1-9]?\d)\.){3}(?:25[0-5]|2[0-4]\d|1\d{2}|[1-9]?\d)$", ErrorMessage = "Invalid IPv4 Address.")]
    string? IPAddress
);