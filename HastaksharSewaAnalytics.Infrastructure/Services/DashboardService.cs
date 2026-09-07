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



    public async Task<List<GetInstallAppDataResponse>> GetHastaksharSewaDailyRunQuery(CancellationToken cancellationToken = default)
    {
        var repo = _unitOfWork.Repository<HastaksharSewaDailyRunLog, int>();
        var todayUtc = DateTime.UtcNow.Date;
        var tomorrowUtc = todayUtc.AddDays(1);
        var rows = await repo.GetListAsync(
            selector: x => new
            {
                x.Id,
                x.DomainId,
                x.IPAddress,
                x.Version,
                x.RunOnDate
            },
            predicate: p => p.RunOnDate >= todayUtc && p.RunOnDate < tomorrowUtc,
            orderBy: q => q.OrderByDescending(x => x.Id),
            cancellationToken: cancellationToken
        );

        var istOffset = TimeSpan.FromHours(5.5);

        return [.. rows.Select(x => new GetInstallAppDataResponse(
            x.Id,
            x.DomainId,
            x.IPAddress,
            x.Version,
            x.RunOnDate.ToOffset(istOffset).ToString("dd-MM-yyyy hh:mm tt")
        ))];
    }

    public async Task<List<GetInstallAppDataResponse>> GetHastaksharSewaInstallationsQuery(
     CancellationToken cancellationToken = default)
    {
        var repo = _unitOfWork.Repository<HastaksharSewaInstallation, int>();

        var rows = await repo.GetListAsync(
            selector: x => new
            {
                x.Id,
                x.DomainId,
                x.IPAddress,
                x.Version,
                x.InstallDate
            },
            predicate: null,
            orderBy: q => q.OrderByDescending(x => x.Id),
            cancellationToken: cancellationToken
        );

        var istOffset = TimeSpan.FromHours(5.5);

        return [.. rows.Select(x => new GetInstallAppDataResponse(
            x.Id,
            x.DomainId,
            x.IPAddress,
            x.Version,
            x.InstallDate.ToOffset(istOffset).ToString("dd-MM-yyyy hh:mm tt")
        ))];
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
            var search = searchValue.Trim();

            query = query.Where(x =>
                (x.IpAddress != null && x.IpAddress.Contains(search)) ||
                (x.MachineName != null && x.MachineName.Contains(search)) ||
                (x.UserName != null && x.UserName.Contains(search)) ||
                (x.OperatingSystem != null && x.OperatingSystem.Contains(search)) ||
                (x.SystemDirectory != null && x.SystemDirectory.Contains(search)) ||
                (x.AppVersion != null && x.AppVersion.Contains(search)) ||
                (x.ErrorMessage != null && x.ErrorMessage.Contains(search))
            );
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

    public async Task<List<GetPublicKeyValtResponse>> GetVaultMasterData(
    CancellationToken cancellationToken = default)
    {
        var repo = _unitOfWork.Repository<VaultMaster, int>();

        var rows = await repo.GetListAsync(
            selector: x => new
            {
                x.Id,
                x.Public_Key,
                x.SerialNo,
                x.TokenValid,
                x.ValidFrom,
                x.ValidTo,
                x.CreatedBy,
                x.CreatedAt
            },
            predicate: null,
            orderBy: o => o.OrderByDescending(x => x.Id),
            cancellationToken: cancellationToken
        );

        var istOffset = TimeSpan.FromHours(5.5);

        return [.. rows.Select(x => new GetPublicKeyValtResponse(
            x.Id,
            x.Public_Key!,
            x.SerialNo!,
            x.TokenValid,
            x.ValidFrom!,
            x.ValidTo!,
            x.CreatedBy!,
            x.CreatedAt.ToOffset(istOffset).ToString("dd-MM-yyyy hh:mm tt")
        ))];
    }

}
