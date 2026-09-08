using HastaksharSewaAnalytics.Application.Abstractions.Interfaces.Common;
using HastaksharSewaAnalytics.Application.Abstractions.Interfaces.IServices;
using HastaksharSewaAnalytics.Application.Dtos.Dashboard;
using HastaksharSewaAnalytics.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace HastaksharSewaAnalytics.Infrastructure.Services;

public sealed record DashboardService : IDashboardService
{
    private readonly IUnitOfWork _unitOfWork;

    public DashboardService(IUnitOfWork unitOfWork) =>
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));

    public async Task<(List<GetInstallAppDataResponse> Data, int TotalCount, int FilteredCount)> GetHastaksharSewaDailyRunQuery(
        int start,
        int length,
        string? searchValue,
        CancellationToken cancellationToken = default)
    {
        start = Math.Max(start, 0);
        length = Math.Clamp(length, 1, 100);

        var repo = _unitOfWork.Repository<HastaksharSewaDailyRunLog, int>();
        var todayUtc = new DateTimeOffset(DateTime.UtcNow.Date, TimeSpan.Zero);
        var tomorrowUtc = todayUtc.AddDays(1);

        var query = repo.Query()
            .Where(x => x.RunOnDate >= todayUtc && x.RunOnDate < tomorrowUtc);

        var totalCount = await query.CountAsync(cancellationToken);

        if (!string.IsNullOrWhiteSpace(searchValue))
        {
            var pattern = $"%{searchValue.Trim()}%";

            query = query.Where(x =>
                (x.DomainId != null && EF.Functions.ILike(x.DomainId, pattern)) ||
                (x.IPAddress != null && EF.Functions.ILike(x.IPAddress, pattern)) ||
                (x.Version != null && EF.Functions.ILike(x.Version, pattern)));
        }

        var filteredCount = await query.CountAsync(cancellationToken);

        var rows = await query
            .OrderByDescending(x => x.Id)
            .Skip(start)
            .Take(length)
            .Select(x => new
            {
                x.Id,
                x.DomainId,
                x.IPAddress,
                x.Version,
                x.RunOnDate
            })
            .ToListAsync(cancellationToken);

        var istOffset = TimeSpan.FromHours(5.5);

        var data = rows.Select(x => new GetInstallAppDataResponse(
            x.Id,
            x.DomainId,
            x.IPAddress,
            x.Version,
            x.RunOnDate.ToOffset(istOffset).ToString("dd-MM-yyyy hh:mm tt")
        )).ToList();

        return (data, totalCount, filteredCount);
    }

    public async Task<(List<GetInstallAppDataResponse> Data, int TotalCount, int FilteredCount)> GetHastaksharSewaInstallationsQuery(
        int start,
        int length,
        string? searchValue,
        CancellationToken cancellationToken = default)
    {
        start = Math.Max(start, 0);
        length = Math.Clamp(length, 1, 100);

        var repo = _unitOfWork.Repository<HastaksharSewaInstallation, int>();
        var query = repo.Query();

        var totalCount = await query.CountAsync(cancellationToken);

        if (!string.IsNullOrWhiteSpace(searchValue))
        {
            var pattern = $"%{searchValue.Trim()}%";

            query = query.Where(x =>
                (x.DomainId != null && EF.Functions.ILike(x.DomainId, pattern)) ||
                (x.IPAddress != null && EF.Functions.ILike(x.IPAddress, pattern)) ||
                (x.Version != null && EF.Functions.ILike(x.Version, pattern)));
        }

        var filteredCount = await query.CountAsync(cancellationToken);

        var rows = await query
            .OrderByDescending(x => x.Id)
            .Skip(start)
            .Take(length)
            .Select(x => new
            {
                x.Id,
                x.DomainId,
                x.IPAddress,
                x.Version,
                x.InstallDate
            })
            .ToListAsync(cancellationToken);

        var istOffset = TimeSpan.FromHours(5.5);

        var data = rows.Select(x => new GetInstallAppDataResponse(
            x.Id,
            x.DomainId,
            x.IPAddress,
            x.Version,
            x.InstallDate.ToOffset(istOffset).ToString("dd-MM-yyyy hh:mm tt")
        )).ToList();

        return (data, totalCount, filteredCount);
    }

    public async Task<int> GetTodayUserCount(CancellationToken cancellationToken = default)
    {
        var repo = _unitOfWork.Repository<HastaksharSewaDailyRunLog, int>();
        var todayStartUtc = new DateTimeOffset(DateTime.UtcNow.Date, TimeSpan.Zero);
        var tomorrowStartUtc = todayStartUtc.AddDays(1);
        return await repo.CountAsync(x => x.RunOnDate >= todayStartUtc && x.RunOnDate < tomorrowStartUtc, cancellationToken);
    }

    public async Task<int> GetTotalInstallationsAsync(CancellationToken cancellationToken = default)
    {
        var repo = _unitOfWork.Repository<HastaksharSewaInstallation, int>();
        return await repo.CountAsync(null, cancellationToken);
    }

    public async Task<int> GetClientErrorLogsCounts(CancellationToken cancellationToken = default)
    {
        var repo = _unitOfWork.Repository<ClientErrorLog, int>();
        return await repo.CountAsync(null, cancellationToken);
    }

    public async Task<(List<GetClientErrorLogsResponse> Data, int TotalCount, int FilteredCount)> GetClientErrorLogsData(
        int start,
        int length,
        string? searchValue,
        CancellationToken cancellationToken = default)
    {
        start = Math.Max(start, 0);
        length = Math.Clamp(length, 1, 100);

        var repo = _unitOfWork.Repository<ClientErrorLog, int>();
        var query = repo.Query();

        var totalCount = await query.CountAsync(cancellationToken);

        if (!string.IsNullOrWhiteSpace(searchValue))
        {
            var pattern = $"%{searchValue.Trim()}%";

            query = query.Where(x =>
                (x.AppName != null && EF.Functions.ILike(x.AppName, pattern)) ||
                (x.ErrorMessage != null && EF.Functions.ILike(x.ErrorMessage, pattern)) ||
                (x.StackTrace != null && EF.Functions.ILike(x.StackTrace, pattern)) ||
                (x.IpAddress != null && EF.Functions.ILike(x.IpAddress, pattern)) ||
                (x.MachineName != null && EF.Functions.ILike(x.MachineName, pattern)) ||
                (x.UserName != null && EF.Functions.ILike(x.UserName, pattern)) ||
                (x.OperatingSystem != null && EF.Functions.ILike(x.OperatingSystem, pattern)) ||
                (x.SystemDirectory != null && EF.Functions.ILike(x.SystemDirectory, pattern)) ||
                (x.AppVersion != null && EF.Functions.ILike(x.AppVersion, pattern)) ||
                (x.Extra != null && EF.Functions.ILike(x.Extra, pattern)));
        }

        var filteredCount = await query.CountAsync(cancellationToken);

        var rows = await query
            .OrderByDescending(x => x.Id)
            .Skip(start)
            .Take(length)
            .Select(x => new
            {
                x.Id,
                x.AppName,
                x.ErrorMessage,
                x.StackTrace,
                x.IpAddress,
                x.MachineName,
                x.UserName,
                x.OperatingSystem,
                x.Is64Bit,
                x.SystemDirectory,
                x.AppVersion,
                x.Extra,
                x.CreatedAt
            })
            .ToListAsync(cancellationToken);

        var istOffset = TimeSpan.FromHours(5.5);

        var data = rows.Select(x => new GetClientErrorLogsResponse(
            x.Id,
            x.AppName,
            x.ErrorMessage!,
            x.StackTrace!,
            x.IpAddress!,
            x.MachineName!,
            x.UserName!,
            x.OperatingSystem!,
            x.Is64Bit,
            x.SystemDirectory!,
            x.AppVersion!,
            x.Extra!,
            x.CreatedAt.ToOffset(istOffset).ToString("dd-MM-yyyy HH:mm")
        )).ToList();

        return (data, totalCount, filteredCount);
    }

    public async Task<int> GetVaultDataCount(CancellationToken cancellationToken = default)
    {
        var repo = _unitOfWork.Repository<VaultMaster, int>();
        return await repo.CountAsync(null, cancellationToken);
    }

    public async Task<(List<GetPublicKeyValtResponse> Data, int TotalCount, int FilteredCount)> GetVaultMasterData(
        int start,
        int length,
        string? searchValue,
        CancellationToken cancellationToken = default)
    {
        start = Math.Max(start, 0);
        length = Math.Clamp(length, 1, 100);

        var repo = _unitOfWork.Repository<VaultMaster, int>();
        var query = repo.Query();

        var totalCount = await query.CountAsync(cancellationToken);

        if (!string.IsNullOrWhiteSpace(searchValue))
        {
            var pattern = $"%{searchValue.Trim()}%";

            query = query.Where(x =>
                (x.SerialNo != null && EF.Functions.ILike(x.SerialNo, pattern)) ||
                (x.Public_Key != null && EF.Functions.ILike(x.Public_Key, pattern)) ||
                (x.ValidFrom != null && EF.Functions.ILike(x.ValidFrom, pattern)) ||
                (x.ValidTo != null && EF.Functions.ILike(x.ValidTo, pattern)) ||
                (x.CreatedBy != null && EF.Functions.ILike(x.CreatedBy, pattern)));
        }

        var filteredCount = await query.CountAsync(cancellationToken);

        var rows = await query
            .OrderByDescending(x => x.Id)
            .Skip(start)
            .Take(length)
            .Select(x => new
            {
                x.Id,
                x.Public_Key,
                x.SerialNo,
                x.TokenValid,
                x.ValidFrom,
                x.ValidTo,
                x.CreatedBy,
                x.CreatedAt
            })
            .ToListAsync(cancellationToken);

        var istOffset = TimeSpan.FromHours(5.5);

        var data = rows.Select(x => new GetPublicKeyValtResponse(
            x.Id,
            x.Public_Key!,
            x.SerialNo!,
            x.TokenValid,
            x.ValidFrom!,
            x.ValidTo!,
            x.CreatedBy!,
            x.CreatedAt.ToOffset(istOffset).ToString("dd-MM-yyyy hh:mm tt")
        )).ToList();

        return (data, totalCount, filteredCount);
    }
}
