using HastaksharSewaAnalytics.Application.Abstractions.Interfaces.Common;
using HastaksharSewaAnalytics.Application.Abstractions.Interfaces.IServices;
using HastaksharSewaAnalytics.Application.Dtos.Dashboard;
using HastaksharSewaAnalytics.Domain.Entities;

namespace HastaksharSewaAnalytics.Infrastructure.Services;

public sealed record DashboardService : IDashboardService
{
    private readonly IUnitOfWork _unitOfWork;
    public DashboardService(IUnitOfWork unitOfWork) =>
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));



    public async Task<List<GetInstallAppDataResponse>> GetHastaksharSewaDailyRunQuery(CancellationToken cancellationToken = default)
    {
        var repo = _unitOfWork.Repository<HastaksharSewaDailyRunLog, Guid>();

        // 1) DB query: only select raw fields (EF-translatable)
        var rows = await repo.GetListAsync(
            selector: x => new
            {
                x.Id,
                x.DomainId,
                x.IPAddress,
                x.Version,
                x.RunOnDate
            },
            predicate: null,
            orderBy: q => q.OrderByDescending(x => x.RunOnDate),
            cancellationToken: cancellationToken
        );

        // 2) In-memory: convert to IST and format
        var istOffset = TimeSpan.FromHours(5.5);

        return rows.Select(x => new GetInstallAppDataResponse(
            x.Id,
            x.DomainId,
            x.IPAddress,
            x.Version,
            x.RunOnDate.ToOffset(istOffset).ToString("dd-MM-yyyy hh:mm tt")
        )).ToList();
    }

    public async Task<List<GetInstallAppDataResponse>> GetHastaksharSewaInstallationsQuery(
     CancellationToken cancellationToken = default)
    {
        var repo = _unitOfWork.Repository<HastaksharSewaInstallation, Guid>();

        // 1) Fetch raw columns only (EF translatable)
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
            orderBy: q => q.OrderByDescending(x => x.InstallDate),
            cancellationToken: cancellationToken
        );

        // 2) Convert/format in-memory (safe)
        var istOffset = TimeSpan.FromHours(5.5);

        return rows.Select(x => new GetInstallAppDataResponse(
            x.Id,
            x.DomainId,
            x.IPAddress,
            x.Version,
            x.InstallDate.ToOffset(istOffset).ToString("dd-MM-yyyy hh:mm tt")
        )).ToList();
    }


    public async Task<int> GetTodayUserCount(CancellationToken cancellationToken = default)
    {
        var repo = _unitOfWork.Repository<HastaksharSewaDailyRunLog, Guid>();
        var todayStartUtc = new DateTimeOffset(DateTime.UtcNow.Date, TimeSpan.Zero);
        var tomorrowStartUtc = todayStartUtc.AddDays(1);
        return await repo.CountAsync(x => x.RunOnDate >= todayStartUtc && x.RunOnDate <tomorrowStartUtc, cancellationToken);
    }

    public async Task<int> GetTotalInstallationsAsync(CancellationToken cancellationToken = default)
    {
        var repo = _unitOfWork.Repository<HastaksharSewaInstallation, Guid>();
        return await repo.CountAsync(null, cancellationToken);
    }

    public async Task<int> GetClientErrorLogsCounts(CancellationToken cancellationToken = default)
    {
        var repo = _unitOfWork.Repository<ClientErrorLog, Guid>();
        return await repo.CountAsync(null, cancellationToken);
    }

    public async Task<List<GetClientErrorLogsResponse>> GetClientErrorLogsData(
    CancellationToken cancellationToken = default)
    {
        var repo = _unitOfWork.Repository<ClientErrorLog, Guid>();

        // 1) Query only raw fields (EF-translatable)
        var rows = await repo.GetListAsync(
            selector: x => new
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
            },
            predicate: null,
            orderBy: q => q.OrderByDescending(x => x.CreatedAt),
            cancellationToken: cancellationToken
        );

        // 2) Convert + format in memory
        // If CreatedAt is DateTimeOffset (best), use offset conversion:
        var istOffset = TimeSpan.FromHours(5.5);

        return rows.Select(x => new GetClientErrorLogsResponse(
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
            // If CreatedAt is DateTimeOffset:
            x.CreatedAt.ToOffset(istOffset).ToString("dd-MM-yyyy HH:mm")
        )).ToList();
    }


    public async Task<int> GetVaultDataCount(CancellationToken cancellationToken = default)
    {
        var repo = _unitOfWork.Repository<VaultMaster, Guid>();
        return await repo.CountAsync(null, cancellationToken);
    }

    public async Task<List<GetPublicKeyValtResponse>> GetVaultMasterData(
    CancellationToken cancellationToken = default)
    {
        var repo = _unitOfWork.Repository<VaultMaster, Guid>();

        // 1) EF query: raw fields only
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
            orderBy: null,
            cancellationToken: cancellationToken
        );

        // 2) In-memory: IST conversion + formatting
        var istOffset = TimeSpan.FromHours(5.5);

        return rows.Select(x => new GetPublicKeyValtResponse(
            x.Id,
            x.Public_Key!,
            x.SerialNo!,
            x.TokenValid,
            x.ValidFrom!,
            x.ValidTo!,
            x.CreatedBy!,
            x.CreatedAt.ToOffset(istOffset).ToString("dd-MM-yyyy hh:mm tt")
        )).ToList();
    }

}
