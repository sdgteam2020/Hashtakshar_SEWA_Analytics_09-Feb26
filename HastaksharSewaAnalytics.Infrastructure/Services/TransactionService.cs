using HastaksharSewaAnalytics.Application.Abstractions.Interfaces.Common;
using HastaksharSewaAnalytics.Application.Abstractions.Interfaces.IServices;
using HastaksharSewaAnalytics.Application.Dtos.Transaction;
using HastaksharSewaAnalytics.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace HastaksharSewaAnalytics.Infrastructure.Services;

public sealed record TransactionService : ITransactionService
{
    private const int VaultSearchResultLimit = 20;
    private const int VaultSearchMinLength = 3;
    private const int VaultSearchMaxLength = 64;
    private readonly IUnitOfWork _unitOfWork;

    public TransactionService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<bool?> SaveVaultMasterData(
        SaveUserPublicDataRequest userPublicDataRequest,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(userPublicDataRequest);

        var repository = _unitOfWork.Repository<VaultMaster, int>();

        var isExist = await repository.CountAsync(
            x => x.SerialNo == userPublicDataRequest.SerialNo,
            ct: cancellationToken);

        if (isExist > 0)
        {
            return true;
        }

        var userPublicData = VaultMaster.Create(
            userPublicDataRequest.Public_Key,
            userPublicDataRequest.SerialNo,
            userPublicDataRequest.TokenValid,
            userPublicDataRequest.ValidFrom,
            userPublicDataRequest.ValidTo
        );

        await repository.AddAsync(userPublicData);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return true;

    }

    public async Task<bool> SaveDailyRunAsync(
        SaveDailyRunRequest saveDailyRunRequest,
        CancellationToken ct = default
    )
    {
        var repository = _unitOfWork.Repository<HastaksharSewaDailyRunLog, int>();
        var todayStartUtc = new DateTimeOffset(DateTime.UtcNow.Date, TimeSpan.Zero);
        var tomorrowStartUtc = todayStartUtc.AddDays(1);

        var isExist = await repository.CountAsync(
            x =>
                x.DomainId == saveDailyRunRequest.DomainId &&
                x.Version == saveDailyRunRequest.Version &&
                x.IPAddress == saveDailyRunRequest.IpAddress &&
                x.CreatedAt >= todayStartUtc &&
                x.CreatedAt < tomorrowStartUtc,
            ct: ct
        );

        if (isExist > 0)
        {
            return true;
        }
        var dailyRunLog = HastaksharSewaDailyRunLog.Create(
           saveDailyRunRequest.DomainId,
           saveDailyRunRequest.IpAddress,
           saveDailyRunRequest.Version,
           saveDailyRunRequest.RunOnDate,
           saveDailyRunRequest.CreatedBy
        );

        await repository.AddAsync(dailyRunLog, ct);
        await _unitOfWork.SaveChangesAsync(ct);
        return true;
    }

    public async Task<bool> SaveInstallationAsync(
       SaveInstallationRquest saveInstallationRquest,
       CancellationToken ct = default
    )
    {
        var repository = _unitOfWork.Repository<HastaksharSewaInstallation, int>();
        var IsExist = await repository.CountAsync(
            x => x.DomainId == saveInstallationRquest.DomainId && x.Version == saveInstallationRquest.Version && x.IPAddress == saveInstallationRquest.IpAddress,
            ct: ct
        );
        if (IsExist > 0)
        {
            return true;
        }
        var installation = HastaksharSewaInstallation.Create(
           saveInstallationRquest.DomainId,
           saveInstallationRquest.IpAddress,
           saveInstallationRquest.Version
        );

        await repository.AddAsync(installation, ct);
        await _unitOfWork.SaveChangesAsync(ct);
        return true;
    }

    public async Task<List<XmlDataForPublicKeyResponse>> SearchVaultMastersBySerialAsync(
        string term,
        CancellationToken ct)
    {
        term = (term ?? string.Empty).Trim();

        if (term.Length < VaultSearchMinLength || term.Length > VaultSearchMaxLength)
            return [];

        var repo = _unitOfWork.Repository<VaultMaster, int>();

        return await repo.Query()
            .Where(x => x.SerialNo != null &&
                        EF.Functions.ILike(x.SerialNo, $"%{term}%"))
            .OrderBy(x => x.SerialNo)
            .Select(x => new XmlDataForPublicKeyResponse
            {
                SerialNo = x.SerialNo,
                Public_Key = x.Public_Key,
                TokenValid = x.TokenValid,
                ValidFrom = x.ValidFrom,
                ValidTo = x.ValidTo,
                Status = true
            })
            .Take(VaultSearchResultLimit)
            .ToListAsync(ct);
    }
}
