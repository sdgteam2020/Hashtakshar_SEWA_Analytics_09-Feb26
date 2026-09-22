using HastaksharSewaAnalytics.Application.Abstractions.Interfaces.Common;
using HastaksharSewaAnalytics.Application.Abstractions.Interfaces.IServices;
using HastaksharSewaAnalytics.Application.Dtos.Transaction;
using HastaksharSewaAnalytics.Domain.Entities;
using HastaksharSewaAnalytics.Infrastructure.Helpers;
using Microsoft.EntityFrameworkCore;

namespace HastaksharSewaAnalytics.Infrastructure.Services;

public sealed record TransactionService : ITransactionService
{
    private const int VaultSearchResultLimit = 20;
    private const int VaultSearchMinLength = 3;
    private const int VaultSearchMaxLength = 64;

    private readonly IUnitOfWork _unitOfWork;
    private readonly MasterDataResolver _masterDataResolver;

    public TransactionService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
        _masterDataResolver = new MasterDataResolver(unitOfWork);
    }

    public async Task<bool?> SaveVaultMasterData(
        SaveUserPublicDataRequest userPublicDataRequest,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(userPublicDataRequest);

        var repository = _unitOfWork.Repository<VaultMaster, int>();
        var existingId = await repository.GetScalarAsync(
            selector: x => x.Id,
            predicate: x => x.SerialNo == userPublicDataRequest.SerialNo,
            cancellationToken: cancellationToken);

        if (existingId > 0)
            return true;

        var validFrom = DateTimeValueParser.ParseRequired(userPublicDataRequest.ValidFrom, nameof(userPublicDataRequest.ValidFrom));
        var validTo = DateTimeValueParser.ParseRequired(userPublicDataRequest.ValidTo, nameof(userPublicDataRequest.ValidTo));

        var vault = VaultMaster.Create(
            userPublicDataRequest.Public_Key,
            userPublicDataRequest.SerialNo,
            userPublicDataRequest.TokenValid,
            validFrom,
            validTo);

        await repository.AddAsync(vault, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<bool> SaveDailyRunAsync(
    SaveDailyRunRequest request,
    CancellationToken ct = default)
    {
        ArgumentNullException.ThrowIfNull(request);


        // Get Client
        var clientId = await _masterDataResolver.GetOrCreateClientAsync(
            request.DomainId,
            request.IpAddress,
            request.CreatedBy,
            ct);


        // Get Version
        var versionId = await _masterDataResolver.GetOrCreateHastaksharVersionAsync(
            request.Version,
            request.CreatedBy,
            ct);


        var installationRepo =
            _unitOfWork.Repository<HastaksharSewaInstallation, int>();


        // Get existing installation
        var installationId = await installationRepo.GetScalarAsync(
            selector: x => x.Id,
            predicate: x =>
                x.CreatedByClientId == clientId &&
                x.VersionId == versionId,
            cancellationToken: ct);


        // Create installation if not exists
        if (installationId == 0)
        {
            var installation =
                HastaksharSewaInstallation.Create(
                    clientId,
                    versionId);

            await installationRepo.AddAsync(
                installation,
                ct);

            await _unitOfWork.SaveChangesAsync(ct);

            installationId = installation.Id;
        }


        var dailyRepo =
            _unitOfWork.Repository<HastaksharSewaDailyRunLog, int>();


        var today = DateOnly.FromDateTime(DateTime.Now);


        var dailyExists = await dailyRepo.CountAsync(
            x =>
                x.CreatedByClientId == installationId &&
                DateOnly.FromDateTime(x.CreatedAt.Date) == today,
            ct);


        if (dailyExists > 0)
            return true;


        var dailyRun =
            HastaksharSewaDailyRunLog.Create(
                installationId);


        await dailyRepo.AddAsync(
            dailyRun,
            ct);


        await _unitOfWork.SaveChangesAsync(ct);


        return true;
    }

    public async Task<bool> SaveInstallationAsync(
        SaveInstallationRquest request,
        CancellationToken ct = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        var clientId = await _masterDataResolver.GetOrCreateClientAsync(
            request.DomainId,
            request.IpAddress,
            createdBy: null,
            ct: ct);

        var versionId = await _masterDataResolver.GetOrCreateHastaksharVersionAsync(
            request.Version,
            createdBy: null,
            ct: ct);

        var repository = _unitOfWork.Repository<HastaksharSewaInstallation, int>();
        var exists = await repository.CountAsync(
            x => x.CreatedByClientId == clientId && x.VersionId == versionId,
            ct);

        if (exists > 0)
            return true;

        var installation = HastaksharSewaInstallation.Create(
            clientId,
            versionId);

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
        var rows = await repo.Query()
            .Where(x => EF.Functions.ILike(x.SerialNo, $"%{term}%"))
            .OrderBy(x => x.SerialNo)
            .Select(x => new
            {
                x.SerialNo,
                x.Public_Key,
                x.TokenValid,
                x.ValidFrom,
                x.ValidTo
            })
            .Take(VaultSearchResultLimit)
            .ToListAsync(ct);

        return rows.Select(x => new XmlDataForPublicKeyResponse
        {
            SerialNo = x.SerialNo,
            Public_Key = x.Public_Key,
            TokenValid = x.TokenValid,
            ValidFrom = DateTimeValueParser.ToApiString(x.ValidFrom),
            ValidTo = DateTimeValueParser.ToApiString(x.ValidTo),
            Status = true
        }).ToList();
    }
}
