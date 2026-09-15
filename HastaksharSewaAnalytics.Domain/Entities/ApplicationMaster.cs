using HastaksharSewaAnalytics.Domain.Premitives.AuditEntity;

namespace HastaksharSewaAnalytics.Domain.Entities;

public sealed class ApplicationMaster : AuditableEntity<int>
{
    private ApplicationMaster() : base(0) { }
    private ApplicationMaster(int id) : base(id) { }

    public string AppCode { get; private set; } = string.Empty;
    public string AppName { get; private set; } = string.Empty;
    public bool IsActive { get; private set; } = true;

    public static ApplicationMaster Create(string appCode, string appName, string? createdBy = null)
    {
        if (string.IsNullOrWhiteSpace(appCode))
            throw new ArgumentException("AppCode is required.", nameof(appCode));
        if (string.IsNullOrWhiteSpace(appName))
            throw new ArgumentException("AppName is required.", nameof(appName));

        appCode = appCode.Trim().ToUpperInvariant();
        appName = appName.Trim();

        if (appCode.Length > 64)
            throw new ArgumentException("AppCode max length is 64.", nameof(appCode));
        var entity = new ApplicationMaster(0)
        {
            AppCode = appCode,
            AppName = appName,
            IsActive = true
        };

        if (!string.IsNullOrWhiteSpace(createdBy))
            entity.SetCreated(createdBy);

        return entity;
    }

    public void Rename(string appName, string modifiedBy)
    {
        if (string.IsNullOrWhiteSpace(appName))
            throw new ArgumentException("AppName is required.", nameof(appName));

        AppName = appName.Trim();
        SetModified(modifiedBy);
    }

    public void Activate(string modifiedBy)
    {
        IsActive = true;
        SetModified(modifiedBy);
    }

    public void Deactivate(string modifiedBy)
    {
        IsActive = false;
        SetModified(modifiedBy);
    }
}
