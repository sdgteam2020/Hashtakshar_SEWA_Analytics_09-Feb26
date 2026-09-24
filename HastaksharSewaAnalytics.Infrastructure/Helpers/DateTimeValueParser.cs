using System.Globalization;

namespace HastaksharSewaAnalytics.Infrastructure.Helpers;

internal static class DateTimeValueParser
{
    public static DateTimeOffset ParseRequired(string value, string fieldName)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException($"{fieldName} is required.", fieldName);


        value = value.Trim();


        string[] formats =
        {
            "dd-MM-yyyy HH:mm:ss zzz",
            "dd-MM-yyyy HH:mm zzz",
            "dd-MM-yyyy HH:mm:ss",
            "yyyy-MM-ddTHH:mm:sszzz",
            "yyyy-MM-ddTHH:mm:ss.fffzzz",
            "O"
        };


        if (DateTimeOffset.TryParseExact(
                value,
                formats,
                CultureInfo.InvariantCulture,
                DateTimeStyles.None,
                out var parsed))
        {
            return parsed.ToUniversalTime(); ;
        }


        throw new ArgumentException(
            $"{fieldName} must be a valid date/time value.",
            fieldName);
    }


    public static DateTimeOffset? ParseOptional(
        string? value,
        string fieldName)
    {
        return string.IsNullOrWhiteSpace(value)
            ? null
            : ParseRequired(value, fieldName);
    }


    public static string ToApiString(DateTimeOffset? value)
      => value?.ToUniversalTime()
          .ToString("O", CultureInfo.InvariantCulture)
          ?? string.Empty;
}