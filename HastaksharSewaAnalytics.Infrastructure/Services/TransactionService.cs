using HastaksharSewaAnalytics.Application.Abstractions.Interfaces.Common;
using HastaksharSewaAnalytics.Application.Abstractions.Interfaces.IServices;
using HastaksharSewaAnalytics.Application.Dtos.Transaction;
using HastaksharSewaAnalytics.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace HastaksharSewaAnalytics.Infrastructure.Services;

public sealed record TransactionService : ITransactionService
{
    private readonly IUnitOfWork _unitOfWork;

    public TransactionService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<bool?> SaveVaultMasterData(SaveUserPublicDataRequest userPublicDataRequest, CancellationToken cancellationToken = default)
    {
        if (userPublicDataRequest == null)
        {
            throw new ArgumentNullException(nameof(userPublicDataRequest));
        }
        try
        {

            var repository = _unitOfWork.Repository<VaultMaster, int>();
            var IsExist = await repository.CountAsync(
               x=> x.SerialNo == userPublicDataRequest.SerialNo,
               ct: cancellationToken
            );
            if (IsExist > 0)
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
        catch
        {
            throw;
        }
    }

    public async Task<bool> SaveDailyRunAsync(
        SaveDailyRunRequest saveDailyRunRequest,
        CancellationToken ct = default
    )
    {
        try
        {

            var repository = _unitOfWork.Repository<HastaksharSewaDailyRunLog, int>();
            var todayStartUtc = new DateTimeOffset(DateTime.UtcNow.Date, TimeSpan.Zero);
            var tomorrowStartUtc = todayStartUtc.AddDays(1);


            var isExist = await repository.CountAsync(
                x =>
                    x.DomainId == saveDailyRunRequest.domainId &&
                    x.Version == saveDailyRunRequest.version &&
                    x.IPAddress == saveDailyRunRequest.ipAddress &&
                    x.CreatedAt >= todayStartUtc &&
                    x.CreatedAt < tomorrowStartUtc,
                ct: ct
            );

            if (isExist > 0)
            {
                return true;
            }
            var dailyRunLog = HastaksharSewaDailyRunLog.Create(
               saveDailyRunRequest.domainId,
               saveDailyRunRequest.ipAddress,
               saveDailyRunRequest.version,
               saveDailyRunRequest.runOnDate,
               saveDailyRunRequest.createdBy
            );

            await repository.AddAsync(dailyRunLog, ct);
            await _unitOfWork.SaveChangesAsync(ct);
            return true;
        }
        catch (Exception)
        {

            throw;
        }
    }

    public async Task<bool> SaveInstallationAsync(
       SaveInstallationRquest saveInstallationRquest,
       CancellationToken ct = default
    )
    {
        try
        {
            var repository = _unitOfWork.Repository<HastaksharSewaInstallation, int>();
            var IsExist = await repository.CountAsync(
                x => x.DomainId == saveInstallationRquest.domainId && x.Version == saveInstallationRquest.version && x.IPAddress == saveInstallationRquest.ipAddress,
                ct: ct
            );
            if (IsExist > 0)
            {
                return true;
            }
            var installation = HastaksharSewaInstallation.Create(
               saveInstallationRquest.domainId,
               saveInstallationRquest.ipAddress,
               saveInstallationRquest.version
            );

            await repository.AddAsync(installation, ct);
            await _unitOfWork.SaveChangesAsync(ct);
            return true;
        }
        catch (Exception)
        {

            throw;
        }
    }

    public async Task<List<XmlDataForPublicKeyResponse>> SearchVaultMastersBySerialAsync(string term, CancellationToken ct)
    {
        term = (term ?? "").Trim();
        if (term.Length == 0) return new List<XmlDataForPublicKeyResponse>();
         
        var repo = _unitOfWork.Repository<VaultMaster, int>();
        var list = await repo.GetListAsync(
            selector: x => new XmlDataForPublicKeyResponse
            {
                SerialNo = x.SerialNo,
                Public_Key = x.Public_Key,
                TokenValid = x.TokenValid,
                ValidFrom = x.ValidFrom,
                ValidTo = x.ValidTo,
                Status = true
            },
            predicate: x => x.SerialNo != null && EF.Functions.ILike(x.SerialNo, $"%{term}%"),
            orderBy: q => q.OrderBy(x => x.SerialNo),
            cancellationToken: ct
        );
         
        return list.Take(20).ToList();
    }
}
