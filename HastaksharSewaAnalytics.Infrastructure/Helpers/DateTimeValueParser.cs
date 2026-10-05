using System.Globalization;

namespace HastaksharSewaAnalytics.Infrastructure.Helpers;

internal static class DateTimeValueParser
{


    public static DateTimeOffset ParseRequired(string value, string fieldName)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException(
                $"{fieldName} is required.",
                fieldName);

        value = value.Trim();

        string[] formats =
        {
            // dd-MM-yyyy
            "dd-MM-yyyy HH:mm:ss zzz",
            "dd-MM-yyyy HH:mm zzz",
            "dd-MM-yyyy HH:mm:ss",
            "dd-MM-yyyy HH:mm",

            // dd/MM/yyyy
            "dd/MM/yyyy HH:mm:ss zzz",
            "dd/MM/yyyy HH:mm zzz",
            "dd/MM/yyyy HH:mm:ss",
            "dd/MM/yyyy HH:mm",

            // d/M/yyyy
            "d/M/yyyy H:mm:ss zzz",
            "d/M/yyyy H:mm zzz",
            "d/M/yyyy H:mm:ss",
            "d/M/yyyy H:mm",

            // yyyy-MM-dd
            "yyyy-MM-dd HH:mm:ss zzz",
            "yyyy-MM-dd HH:mm zzz",
            "yyyy-MM-dd HH:mm:ss",
            "yyyy-MM-dd HH:mm",

            // yyyy/MM/dd
            "yyyy/MM/dd HH:mm:ss zzz",
            "yyyy/MM/dd HH:mm zzz",
            "yyyy/MM/dd HH:mm:ss",
            "yyyy/MM/dd HH:mm",

            // ISO
            "yyyy-MM-ddTHH:mm:sszzz",
            "yyyy-MM-ddTHH:mm:ss.fffzzz",
            "yyyy-MM-ddTHH:mm:ss",
            "yyyy-MM-ddTHH:mm:ss.fff",

            // Round-trip ISO
            "O"
        };

        // First try known safe formats
        if (DateTimeOffset.TryParseExact(
                value,
                formats,
                CultureInfo.InvariantCulture,
                DateTimeStyles.AllowWhiteSpaces,
                out DateTimeOffset parsed))
        {
            return parsed.ToUniversalTime();
        }

        // Then try more flexible Indian date parsing
        if (DateTimeOffset.TryParse(
                value,
                CultureInfo.GetCultureInfo("en-IN"),
                DateTimeStyles.AllowWhiteSpaces,
                out parsed))
        {
            return parsed.ToUniversalTime();
        }

        // Finally try invariant parsing
        if (DateTimeOffset.TryParse(
                value,
                CultureInfo.InvariantCulture,
                DateTimeStyles.AllowWhiteSpaces,
                out parsed))
        {
            return parsed.ToUniversalTime();
        }

        throw new ArgumentException(
            $"{fieldName} must be a valid date/time value. Received: '{value}'",
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