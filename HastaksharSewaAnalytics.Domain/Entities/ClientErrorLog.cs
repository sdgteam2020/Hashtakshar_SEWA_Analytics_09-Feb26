using HastaksharSewaAnalytics.Domain.Premitives.AuditEntity;

namespace HastaksharSewaAnalytics.Domain.Entities;

public sealed class ClientErrorLog : AuditableEntity<int>
{
    private ClientErrorLog() : base(0) { }
    private ClientErrorLog(int id) : base(id) { }

    public int ClientId { get; private set; }
    public int ApplicationVersionId { get; private set; }
    public Guid? RequestId { get; private set; }

    public string? MachineName { get; private set; }
    public string? UserName { get; private set; }
    public string? OperatingSystem { get; private set; }
    public bool Is64Bit { get; private set; }
    public string? SystemDirectory { get; private set; }

    public string ErrorMessage { get; private set; } = string.Empty;
    public string? StackTrace { get; private set; }
    public string? Extra { get; private set; }

    public bool IsResolved { get; private set; }
    public DateTimeOffset? ResolvedAtUtc { get; private set; }
    public string? ResolutionNote { get; private set; }

    public static ClientErrorLog Create(
        int clientId,
        int applicationVersionId,
        string errorMessage,
        string? stackTrace,
        string machineName,
        string? userName,
        string operatingSystem,
        bool is64Bit,
        string? systemDirectory,
        string? extra = null,
        Guid? requestId = null)
    {
        if (clientId <= 0)
            throw new ArgumentOutOfRangeException(nameof(clientId));
        if (applicationVersionId <= 0)
            throw new ArgumentOutOfRangeException(nameof(applicationVersionId));
        if (string.IsNullOrWhiteSpace(errorMessage))
            throw new ArgumentException("ErrorMessage is required.", nameof(errorMessage));

        return new ClientErrorLog(0)
        {
            ClientId = clientId,
            ApplicationVersionId = applicationVersionId,
            RequestId = requestId,
            ErrorMessage = Truncate(errorMessage.Trim(), 4000),
            StackTrace = Truncate(stackTrace, 8000),
            MachineName = Normalize(machineName, 128),
            UserName = Normalize(userName, 128),
            OperatingSystem = Normalize(operatingSystem, 256),
            Is64Bit = is64Bit,
            SystemDirectory = Normalize(systemDirectory, 256),
            Extra = Truncate(extra, 2000),
            IsResolved = false
        };
    }

    public void AttachExtra(string extra)
    {
        if (string.IsNullOrWhiteSpace(extra)) return;
        var append = extra.Trim();
        Extra = Truncate(string.IsNullOrWhiteSpace(Extra) ? append : $"{Extra}\n{append}", 2000);
    }

    public void ReplaceStackTrace(string? stackTrace) => StackTrace = Truncate(stackTrace, 8000);

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

    private static string? Normalize(string? value, int maxLen)
        => string.IsNullOrWhiteSpace(value) ? null : Truncate(value.Trim(), maxLen);

    private static string? Truncate(string? value, int maxLen)
    {
        if (string.IsNullOrEmpty(value)) return value;
        return value.Length <= maxLen ? value : value[..maxLen];
    }
}
