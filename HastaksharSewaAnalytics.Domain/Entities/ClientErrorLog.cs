using HastaksharSewaAnalytics.Domain.Premitives.AuditEntity;
namespace HastaksharSewaAnalytics.Domain.Entities;

public sealed class ClientErrorLog : AuditableEntity<Guid>
{
    // State is protected
    public string AppName { get; private set; } = default!;
    public string? AppVersion { get; private set; }

    public string? IpAddress { get; private set; }
    public string? MachineName { get; private set; }
    public string? UserName { get; private set; }
    public string? OperatingSystem { get; private set; }
    public bool Is64Bit { get; private set; }
    public string? SystemDirectory { get; private set; }

    public string? ErrorMessage { get; private set; } = default!;
    public string? StackTrace { get; private set; }
    public string? Extra { get; private set; }

    public bool IsResolved { get; private set; }
    public DateTimeOffset? ResolvedAtUtc { get; private set; }
    public string? ResolutionNote { get; private set; }

    // EF Core needs a parameterless constructor
    private ClientErrorLog() : base(Guid.Empty) { }

    private ClientErrorLog(Guid id) : base(id) { }

    // ---------- FACTORY ----------
    public static ClientErrorLog Create(
        string appName,
        string errorMessage,
        string? stackTrace,
        string ipaddress,
        string machineName,
        string? userName,
        string operatingSystem,
        bool is64Bit,
        string? systemDirectory,
        string? appVersion = null,
        string? extra = null)
    {
        if (string.IsNullOrWhiteSpace(appName))
            throw new ArgumentException("AppName is required.", nameof(appName));

        if (string.IsNullOrWhiteSpace(errorMessage))
            throw new ArgumentException("ErrorMessage is required.", nameof(errorMessage));

        var log = new ClientErrorLog(Guid.NewGuid())
        {
            AppName = appName.Trim(),
            AppVersion = Normalize(appVersion, 50),
            ErrorMessage = Truncate(errorMessage.Trim(), 4000),
            StackTrace = Truncate(stackTrace, 8000),

            IpAddress = Normalize(ipaddress, 64),
            MachineName = Normalize(machineName, 128),
            UserName = Normalize(userName, 128),
            OperatingSystem = Normalize(operatingSystem, 256),
            Is64Bit = is64Bit,
            SystemDirectory = Normalize(systemDirectory, 256),

            Extra = Truncate(extra, 2000),

            IsResolved = false,
            ResolvedAtUtc = null,
            ResolutionNote = null
        };

        return log;
    }

    // ---------- BEHAVIOURS ----------
    public void AttachExtra(string extra)
    {
        if (string.IsNullOrWhiteSpace(extra)) return;

        // Append safely (avoid unbounded growth)
        var append = extra.Trim();
        var combined = string.IsNullOrWhiteSpace(Extra) ? append : $"{Extra}\n{append}";
        Extra = Truncate(combined, 2000);
    }

    public void ReplaceStackTrace(string? stackTrace)
    {
        StackTrace = Truncate(stackTrace, 8000);
    }

    public void MarkResolved(string? note = null)
    {
        IsResolved = true;
        ResolvedAtUtc = DateTimeOffset.UtcNow;
        ResolutionNote = Truncate(note, 1000);
    }

    public void Reopen(string? note = null)
    {
        IsResolved = false;
        ResolvedAtUtc = null;
        ResolutionNote = Truncate(note, 1000);
    }

    // ---------- HELPERS ----------
    private static string? Normalize(string? value, int maxLen)
        => string.IsNullOrWhiteSpace(value) ? null : Truncate(value.Trim(), maxLen);

    private static string? Truncate(string? value, int maxLen)
    {
        if (string.IsNullOrEmpty(value)) return value;
        return value.Length <= maxLen ? value : value.Substring(0, maxLen);
    }
}

