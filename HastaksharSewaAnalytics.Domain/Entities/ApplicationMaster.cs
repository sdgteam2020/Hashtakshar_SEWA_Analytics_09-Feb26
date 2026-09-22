using HastaksharSewaAnalytics.Domain.Premitives.AuditEntity;
using HastaksharSewaAnalytics.Domain.Premitives.Enums;

namespace HastaksharSewaAnalytics.Domain.Entities;

public sealed class ApplicationMaster : AuditableEntity<int>
{
    private ApplicationMaster() : base(0) { }
    private ApplicationMaster(int id) : base(id) { }

    public string AppName { get; private set; } = string.Empty;
    public bool IsActive { get; private set; } = true;

    public static ApplicationMaster Create(string appName)
    {
        if (string.IsNullOrWhiteSpace(appName))
            throw new ArgumentException("AppName is required.", nameof(appName));

        appName = appName.Trim();

        var entity = new ApplicationMaster(0)
        {
            AppName = appName,
            IsActive = true
        };

        entity.SetCreated(GlobalVariables.UserId);

        return entity;
    }

    public void Rename(string appName)
    {
        if (string.IsNullOrWhiteSpace(appName))
            throw new ArgumentException("AppName is required.", nameof(appName));

        AppName = appName.Trim();
    }


    public void Activate()
    {
        IsActive = true;
    }

    public void Deactivate()
    {
        IsActive = false;
    }
}
