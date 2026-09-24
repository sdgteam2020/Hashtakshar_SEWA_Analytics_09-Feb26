using HastaksharSewaAnalytics.Application.Abstractions.Interfaces.Common;
using HastaksharSewaAnalytics.Application.Abstractions.Interfaces.IServices;
using HastaksharSewaAnalytics.Application.Dtos.DigitalSign;
using HastaksharSewaAnalytics.Domain.Entities;
using HastaksharSewaAnalytics.Infrastructure.Helpers;
using Microsoft.EntityFrameworkCore;

namespace HastaksharSewaAnalytics.Infrastructure.Services;

public sealed record DigitalSignService : IDigitalSignService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly MasterDataResolver _masterDataResolver;
    private readonly TimeSpan _istOffset = TimeSpan.FromHours(5.5);

    public DigitalSignService(IUnitOfWork unitOfWork, MasterDataResolver masterDataResolver)
    {
        _unitOfWork = unitOfWork;
        _masterDataResolver = masterDataResolver;
    }

    public async Task<int> GetDigitalSignCountAsync(CancellationToken cancellationToken = default)
        => await _unitOfWork.Repository<DigitalSignDetail, int>()
            .CountAsync(null, cancellationToken);

    public async Task<(List<GetDigitalSignRespone> Data, int TotalCount, int FilteredCount)> GetDigitalSignListAsync(
        int start,
        int length,
        string? searchValue,
        CancellationToken cancellationToken = default)
    {
        start = Math.Max(start, 0);
        length = Math.Clamp(length, 1, 100);

        var digitalSign = _unitOfWork.Repository<DigitalSignDetail, int>().Query();
        var client = _unitOfWork.Repository<ClientMaster, int>().Query();
        var query = from sign in digitalSign
                    join c in client on sign.CreatedByClientId equals c.Id
                    select new
                    {
                        sign.Id,
                        sign.VaultMasterId,
                        sign.DocumentName,
                        sign.SignDateTime,
                        c.IPAddress
                    };
        var totalCount = await query.CountAsync(cancellationToken);

        if (!string.IsNullOrWhiteSpace(searchValue))
        {
            var search = searchValue.Trim();
            var pattern = $"%{search}%";

            if (int.TryParse(search, out var vaultMasterId))
            {
                query = query.Where(x =>
                    x.VaultMasterId == vaultMasterId ||
                    (x.DocumentName != null && EF.Functions.ILike(x.DocumentName, pattern)));
            }
            else
            {
                query = query.Where(x =>
                    (x.DocumentName != null && EF.Functions.ILike(x.DocumentName, pattern)));
            }
        }

        var filteredCount = await query.CountAsync(cancellationToken);
        var rows = await query
            .OrderByDescending(x => x.Id)
            .Skip(start)
            .Take(length)
            .Select(x => new
            {
                x.Id,
                x.VaultMasterId,
                x.DocumentName,
                x.SignDateTime,
                x.IPAddress
            })
            .ToListAsync(cancellationToken);

        var data = rows.Select(x => new GetDigitalSignRespone(
            x.Id,
            x.VaultMasterId,
            x.IPAddress,
            x.DocumentName ?? string.Empty,
            x.SignDateTime.ToOffset(_istOffset).ToString("dd-MM-yyyy hh:mm tt")
        )).ToList();

        return (data, totalCount, filteredCount);
    }

    public async Task<bool> SaveDigitalSign(
     SaveDigitalSignRequest request,
     CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        cancellationToken.ThrowIfCancellationRequested();

        var vaultRepo =
            _unitOfWork.Repository<VaultMaster, int>();

        var digitalSignRepo =
            _unitOfWork.Repository<DigitalSignDetail, int>();

        var vaultMasterId = await vaultRepo.GetScalarAsync(
            selector: x => x.Id,
            predicate: x => x.SerialNo == request.SerialNo,
            cancellationToken: cancellationToken
        );

        cancellationToken.ThrowIfCancellationRequested();

        var clientId =
            await _masterDataResolver.GetOrCreateClientAsync(
                null,
                request.IPAddress,
                createdBy: null,
                ct: cancellationToken
            );

        cancellationToken.ThrowIfCancellationRequested();

        if (clientId <= 0)
            throw new InvalidOperationException(
                "Unable to resolve client ID.");

        if (vaultMasterId <= 0)
        {
            var validFrom =
                DateTimeValueParser.ParseRequired(
                    request.ValidFrom,
                    nameof(request.ValidFrom));

            var validTo =
                DateTimeValueParser.ParseRequired(
                    request.ValidTo,
                    nameof(request.ValidTo));

            var vault = VaultMaster.Create(
                request.PublicKey,
                request.SerialNo,
                request.TokenValid,
                validFrom,
                validTo);

            vault.SetCreated(clientId);

            await vaultRepo.AddAsync(
                vault,
                cancellationToken);

            await _unitOfWork.SaveChangesAsync(
                cancellationToken);

            vaultMasterId = vault.Id;

            if (vaultMasterId <= 0)
                throw new InvalidOperationException(
                    "VaultMaster was not created successfully.");
        }

        cancellationToken.ThrowIfCancellationRequested();

        var signDateTime =
            DateTimeValueParser.ParseRequired(
                request.SignedDateTime,
                nameof(request.SignedDateTime));

        var entity = DigitalSignDetail.Create(
            vaultMasterId,
            clientId,
            signDateTime,
            request.DocumentName);

        await digitalSignRepo.AddAsync(
            entity,
            cancellationToken);

        var affectedRows =
            await _unitOfWork.SaveChangesAsync(
                cancellationToken);

        return affectedRows > 0;
    }
}
