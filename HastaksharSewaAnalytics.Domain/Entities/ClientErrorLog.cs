using HastaksharSewaAnalytics.Domain.Premitives.AuditEntity;

namespace HastaksharSewaAnalytics.Domain.Entities;

public sealed class ClientErrorLog : ClientAuditableEntity<int>
{
    private ClientErrorLog() : base(0) { }
    private ClientErrorLog(int id) : base(id) { }

    public int ApplicationVersionId { get; private set; }    
    public string ErrorMessage { get; private set; } = string.Empty;

    public static ClientErrorLog Create(
        int clientId,
        int applicationVersionId,
        string errorMessage
        )
    {
        if (clientId <= 0)
            throw new ArgumentOutOfRangeException(nameof(clientId));
        if (applicationVersionId <= 0)
            throw new ArgumentOutOfRangeException(nameof(applicationVersionId));
        if (string.IsNullOrWhiteSpace(errorMessage))
            throw new ArgumentException("ErrorMessage is required.", nameof(errorMessage));

        var entity = new ClientErrorLog(0);
        entity.SetCreated(clientId);
        entity.ApplicationVersionId = applicationVersionId;
        entity.ErrorMessage = Truncate(errorMessage.Trim(), 4000)!;
        return entity;
    }

    public void AttachExtra(string extra)
    {
        if (string.IsNullOrWhiteSpace(extra)) return;
        var append = extra.Trim();
    }

    private static string? Truncate(string? value, int maxLen)
    {
        if (string.IsNullOrEmpty(value)) return value;
        return value.Length <= maxLen ? value : value[..maxLen];
    }
}
