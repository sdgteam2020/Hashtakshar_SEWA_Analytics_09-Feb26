using System.Globalization;

namespace HastaksharSewaAnalytics.Infrastructure.Helpers;

internal static class DateTimeValueParser
{
    public static DateTimeOffset ParseRequired(string value, string fieldName)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException($"{fieldName} is required.", fieldName);

        if (!DateTimeOffset.TryParse(
                value.Trim(),
                CultureInfo.InvariantCulture,
                DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal,
                out var parsed))
        {
            throw new ArgumentException($"{fieldName} must be a valid date/time value.", fieldName);
        }

        return parsed;
    }

    public static DateTimeOffset? ParseOptional(string? value, string fieldName)
        => string.IsNullOrWhiteSpace(value) ? null : ParseRequired(value, fieldName);

    public static string ToApiString(DateTimeOffset? value)
        => value?.ToUniversalTime().ToString("O", CultureInfo.InvariantCulture) ?? string.Empty;
}
