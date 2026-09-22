using HastaksharSewaAnalytics.Domain.Premitives.AuditEntity;

namespace HastaksharSewaAnalytics.Domain.Entities;

public sealed class HastaksharSewaDailyRunLog : ClientAuditableEntity<int>
{
    private HastaksharSewaDailyRunLog() : base(0) { }
    private HastaksharSewaDailyRunLog(int id) : base(id) { }      

    public static HastaksharSewaDailyRunLog Create(
        int installedId)
    {
        if (installedId <= 0)
            throw new ArgumentOutOfRangeException(nameof(installedId));

        var entity = new HastaksharSewaDailyRunLog(0);

        entity.SetCreated(installedId);
        
        return entity;
    }
}
