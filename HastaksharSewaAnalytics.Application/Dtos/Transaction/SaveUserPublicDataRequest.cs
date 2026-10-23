using System.ComponentModel.DataAnnotations;
namespace HastaksharSewaAnalytics.Application.Dtos.Transaction;


public record SaveUserPublicDataRequest(

    // Public Key
    [property: Required(ErrorMessage = "Public Key is required.")]
    [property: StringLength(32768, ErrorMessage = "Public Key exceeds the maximum allowed length.")]
    string Public_Key,

    // Certificate Serial Number (Hexadecimal)
    [property: Required(ErrorMessage = "Serial Number is required.")]
    [property: RegularExpression(@"^[A-Fa-f0-9]{1,64}$", ErrorMessage = "Invalid Serial Number format.")]
    string SerialNo,

    // Token Validity (Boolean - No Regex Required)
    bool TokenValid,

    // Certificate Valid From
    [property: Required(ErrorMessage = "Valid From is required.")]
    [property: RegularExpression(@"^\d{4}-\d{2}-\d{2}T\d{2}:\d{2}:\d{2}(?:\.\d{1,7})?(?:Z|[+-]\d{2}:\d{2})?$", ErrorMessage = "Invalid Valid From date format.")]
    string ValidFrom,

    // Certificate Valid To
    [property: Required(ErrorMessage = "Valid To is required.")]
    [property: RegularExpression(@"^\d{4}-\d{2}-\d{2}T\d{2}:\d{2}:\d{2}(?:\.\d{1,7})?(?:Z|[+-]\d{2}:\d{2})?$", ErrorMessage = "Invalid Valid To date format.")]
    string ValidTo
);
