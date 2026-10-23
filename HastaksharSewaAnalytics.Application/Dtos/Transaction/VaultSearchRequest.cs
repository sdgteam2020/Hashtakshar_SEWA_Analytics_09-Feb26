using System.ComponentModel.DataAnnotations;
namespace HastaksharSewaAnalytics.Application.Dtos.Transaction;

public sealed record VaultSearchRequest
{
    // Army Number (Optional)
    // Supports partial Army No, full Army No, and 64-character hash
    [StringLength(64,
        ErrorMessage = "Army Number cannot exceed 64 characters.")]
    [RegularExpression(
        @"^[A-Za-z0-9]{1,64}$",
        ErrorMessage = "Invalid Army Number search format.")]
    public string? ArmyNo { get; set; }

    // Name (Optional)
    // Supports partial and full name searches
    [StringLength(100,
        ErrorMessage = "Name cannot exceed 100 characters.")]
    [RegularExpression(
        @"^[\p{L}][\p{L}\p{M} .'-]{0,99}$",
        ErrorMessage = "Invalid Name search format.")]
    public string? Name { get; set; }

    // Search Term (Optional)
    // Supports partial and full term searches
    [StringLength(200,
        ErrorMessage = "Search Term cannot exceed 200 characters.")]
    [RegularExpression(
        @"^[\p{L}\p{N} .,_'()/&@+#:-]{1,200}$",
        ErrorMessage = "Invalid Search Term format.")]
    public string? Term { get; set; }
}
