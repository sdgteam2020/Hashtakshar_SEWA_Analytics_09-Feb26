using HastaksharSewaAnalytics.Application.Abstractions.Interfaces.Common;
using HastaksharSewaAnalytics.Application.Abstractions.Interfaces.IServices;
using HastaksharSewaAnalytics.Application.Dtos.Dashboard;
using HastaksharSewaAnalytics.Domain.Entities;
using HastaksharSewaAnalytics.Infrastructure.Helpers;
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

        var logs = _unitOfWork.Repository<HastaksharSewaDailyRunLog, int>().Query();
        var clients = _unitOfWork.Repository<ClientMaster, int>().Query();
        var versions = _unitOfWork.Repository<ApplicationVersion, int>().Query();
        var todayUtc = DateOnly.FromDateTime(DateTime.UtcNow);

        var query = from log in logs
                    join client in clients on log.ClientId equals client.Id
                    join version in versions on log.VersionId equals version.Id
                    where log.RunOnDate == todayUtc
                    select new
                    {
                        log.Id,
                        client.DomainId,
                        client.IPAddress,
                        version.Version,
                        log.RunOnDate
                    };

        var totalCount = await query.CountAsync(cancellationToken);

        if (!string.IsNullOrWhiteSpace(searchValue))
        {
            var pattern = $"%{searchValue.Trim()}%";
            query = query.Where(x =>
                EF.Functions.ILike(x.DomainId, pattern) ||
                EF.Functions.ILike(x.IPAddress, pattern) ||
                EF.Functions.ILike(x.Version, pattern));
        }

        var filteredCount = await query.CountAsync(cancellationToken);
        var rows = await query
            .OrderByDescending(x => x.Id)
            .Skip(start)
            .Take(length)
            .ToListAsync(cancellationToken);

        var data = rows.Select(x => new GetInstallAppDataResponse(
            x.Id,
            x.DomainId,
            x.IPAddress,
            x.Version,
            x.RunOnDate.ToString("dd-MM-yyyy")
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

        var installations = _unitOfWork.Repository<HastaksharSewaInstallation, int>().Query();
        var clients = _unitOfWork.Repository<ClientMaster, int>().Query();
        var versions = _unitOfWork.Repository<ApplicationVersion, int>().Query();

        var query = from installation in installations
                    join client in clients on installation.ClientId equals client.Id
                    join version in versions on installation.VersionId equals version.Id
                    select new
                    {
                        installation.Id,
                        client.DomainId,
                        client.IPAddress,
                        version.Version,
                        installation.InstallDate
                    };

        var totalCount = await query.CountAsync(cancellationToken);

        if (!string.IsNullOrWhiteSpace(searchValue))
        {
            var pattern = $"%{searchValue.Trim()}%";
            query = query.Where(x =>
                EF.Functions.ILike(x.DomainId, pattern) ||
                EF.Functions.ILike(x.IPAddress, pattern) ||
                EF.Functions.ILike(x.Version, pattern));
        }

        var filteredCount = await query.CountAsync(cancellationToken);
        var rows = await query
            .OrderByDescending(x => x.Id)
            .Skip(start)
            .Take(length)
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
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        return await _unitOfWork.Repository<HastaksharSewaDailyRunLog, int>()
            .CountAsync(x => x.RunOnDate == today, cancellationToken);
    }

    public async Task<int> GetTotalInstallationsAsync(CancellationToken cancellationToken = default)
        => await _unitOfWork.Repository<HastaksharSewaInstallation, int>()
            .CountAsync(null, cancellationToken);

    public async Task<int> GetClientErrorLogsCounts(CancellationToken cancellationToken = default)
        => await _unitOfWork.Repository<ClientErrorLog, int>()
            .CountAsync(null, cancellationToken);

    public async Task<(List<GetClientErrorLogsResponse> Data, int TotalCount, int FilteredCount)> GetClientErrorLogsData(
        int start,
        int length,
        string? searchValue,
        CancellationToken cancellationToken = default)
    {
        start = Math.Max(start, 0);
        length = Math.Clamp(length, 1, 100);

        var errorLogs = _unitOfWork.Repository<ClientErrorLog, int>().Query();
        var clients = _unitOfWork.Repository<ClientMaster, int>().Query();
        var versions = _unitOfWork.Repository<ApplicationVersion, int>().Query();
        var applications = _unitOfWork.Repository<ApplicationMaster, int>().Query();

        var query = from log in errorLogs
                    join client in clients on log.ClientId equals client.Id
                    join version in versions on log.ApplicationVersionId equals version.Id
                    join app in applications on version.ApplicationId equals app.Id
                    select new
                    {
                        log.Id,
                        app.AppName,
                        log.ErrorMessage,
                        log.StackTrace,
                        IpAddress = client.IPAddress,
                        log.MachineName,
                        log.UserName,
                        log.OperatingSystem,
                        log.Is64Bit,
                        log.SystemDirectory,
                        AppVersion = version.Version,
                        log.Extra,
                        log.CreatedAt
                    };

        var totalCount = await query.CountAsync(cancellationToken);

        if (!string.IsNullOrWhiteSpace(searchValue))
        {
            var pattern = $"%{searchValue.Trim()}%";
            query = query.Where(x =>
                EF.Functions.ILike(x.AppName, pattern) ||
                (x.ErrorMessage != null && EF.Functions.ILike(x.ErrorMessage, pattern)) ||
                (x.StackTrace != null && EF.Functions.ILike(x.StackTrace, pattern)) ||
                EF.Functions.ILike(x.IpAddress, pattern) ||
                (x.MachineName != null && EF.Functions.ILike(x.MachineName, pattern)) ||
                (x.UserName != null && EF.Functions.ILike(x.UserName, pattern)) ||
                (x.OperatingSystem != null && EF.Functions.ILike(x.OperatingSystem, pattern)) ||
                (x.SystemDirectory != null && EF.Functions.ILike(x.SystemDirectory, pattern)) ||
                EF.Functions.ILike(x.AppVersion, pattern) ||
                (x.Extra != null && EF.Functions.ILike(x.Extra, pattern)));
        }

        var filteredCount = await query.CountAsync(cancellationToken);
        var rows = await query
            .OrderByDescending(x => x.Id)
            .Skip(start)
            .Take(length)
            .ToListAsync(cancellationToken);

        var istOffset = TimeSpan.FromHours(5.5);
        var data = rows.Select(x => new GetClientErrorLogsResponse(
            x.Id,
            x.AppName,
            x.ErrorMessage ?? string.Empty,
            x.StackTrace ?? string.Empty,
            x.IpAddress,
            x.MachineName ?? string.Empty,
            x.UserName ?? string.Empty,
            x.OperatingSystem ?? string.Empty,
            x.Is64Bit,
            x.SystemDirectory ?? string.Empty,
            x.AppVersion,
            x.Extra ?? string.Empty,
            x.CreatedAt.ToOffset(istOffset).ToString("dd-MM-yyyy HH:mm")
        )).ToList();

        return (data, totalCount, filteredCount);
    }

    public async Task<int> GetVaultDataCount(CancellationToken cancellationToken = default)
        => await _unitOfWork.Repository<VaultMaster, int>()
            .CountAsync(null, cancellationToken);

    public async Task<(List<GetPublicKeyValtResponse> Data, int TotalCount, int FilteredCount)> GetVaultMasterData(
        int start,
        int length,
        string? searchValue,
        CancellationToken cancellationToken = default)
    {
        start = Math.Max(start, 0);
        length = Math.Clamp(length, 1, 100);

        var query = _unitOfWork.Repository<VaultMaster, int>().Query();
        var totalCount = await query.CountAsync(cancellationToken);

        if (!string.IsNullOrWhiteSpace(searchValue))
        {
            var pattern = $"%{searchValue.Trim()}%";
            query = query.Where(x =>
                EF.Functions.ILike(x.SerialNo, pattern) ||
                EF.Functions.ILike(x.Public_Key, pattern) ||
                EF.Functions.ILike(x.CreatedBy, pattern));
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
            x.Public_Key,
            x.SerialNo,
            x.TokenValid,
            DateTimeValueParser.ToApiString(x.ValidFrom),
            DateTimeValueParser.ToApiString(x.ValidTo),
            x.CreatedBy,
            x.CreatedAt.ToOffset(istOffset).ToString("dd-MM-yyyy hh:mm tt")
        )).ToList();

        return (data, totalCount, filteredCount);
    }
}
