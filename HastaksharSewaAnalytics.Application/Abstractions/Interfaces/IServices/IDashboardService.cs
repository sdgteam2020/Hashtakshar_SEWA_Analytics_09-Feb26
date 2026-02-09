using HastaksharSewaAnalytics.Application.Dtos.Dashboard;

namespace HastaksharSewaAnalytics.Application.Abstractions.Interfaces.IServices;

public interface IDashboardService
{
    Task<int> GetTotalInstallationsAsync(CancellationToken cancellationToken = default);
    Task<int> GetTodayUserCount(CancellationToken cancellationToken = default);
    Task<List<GetInstallAppDataResponse>> GetHastaksharSewaInstallationsQuery(CancellationToken cancellationToken=default);
    Task<List<GetInstallAppDataResponse>> GetHastaksharSewaDailyRunQuery(CancellationToken cancellationToken=default);
    Task<int> GetClientErrorLogsCounts(CancellationToken cancellationToken = default);
    Task<List<GetClientErrorLogsResponse>> GetClientErrorLogsData(CancellationToken cancellationToken = default);
    Task<int> GetVaultDataCount(CancellationToken cancellationToken = default);
    Task<List<GetPublicKeyValtResponse>> GetVaultMasterData(CancellationToken cancellationToken = default);
}
