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

    public DigitalSignService(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

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

        var query = _unitOfWork.Repository<DigitalSignDetail, int>().Query();
        var totalCount = await query.CountAsync(cancellationToken);

        if (!string.IsNullOrWhiteSpace(searchValue))
        {
            var search = searchValue.Trim();
            var pattern = $"%{search}%";

            if (int.TryParse(search, out var vaultMasterId))
            {
                query = query.Where(x =>
                    x.VaultMasterId == vaultMasterId ||
                    (x.DocumentName != null && EF.Functions.ILike(x.DocumentName, pattern)) ||
                    (x.IpAddress != null && EF.Functions.ILike(x.IpAddress, pattern)));
            }
            else
            {
                query = query.Where(x =>
                    (x.DocumentName != null && EF.Functions.ILike(x.DocumentName, pattern)) ||
                    (x.IpAddress != null && EF.Functions.ILike(x.IpAddress, pattern)));
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
                x.IpAddress
            })
            .ToListAsync(cancellationToken);

        var data = rows.Select(x => new GetDigitalSignRespone(
            x.Id,
            x.VaultMasterId,
            x.DocumentName ?? string.Empty,
            DateTimeValueParser.ToApiString(x.SignDateTime),
            x.IpAddress ?? string.Empty
        )).ToList();

        return (data, totalCount, filteredCount);
    }

    public async Task<bool> SaveDigitalSign(
        SaveDigitalSignRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        var vaultRepo = _unitOfWork.Repository<VaultMaster, int>();
        var digitalSignRepo = _unitOfWork.Repository<DigitalSignDetail, int>();

        var vaultMasterId = await vaultRepo.GetScalarAsync(
            selector: x => x.Id,
            predicate: x => x.SerialNo == request.SerialNo,
            cancellationToken: cancellationToken);

        if (vaultMasterId <= 0)
        {
            var validFrom = DateTimeValueParser.ParseRequired(request.ValidFrom, nameof(request.ValidFrom));
            var validTo = DateTimeValueParser.ParseRequired(request.ValidTo, nameof(request.ValidTo));

            var vault = VaultMaster.Create(
                request.PublicKey,
                request.SerialNo,
                request.TokenValid,
                validFrom,
                validTo);

            await vaultRepo.AddAsync(vault, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            vaultMasterId = vault.Id;
        }

        var signDateTime = DateTimeValueParser.ParseRequired(request.SignedDateTime, nameof(request.SignedDateTime));
        var entity = DigitalSignDetail.Create(
            vaultMasterId,
            signDateTime,
            request.OriginForSign,
            request.RefererForSign,
            request.IpAddress,
            request.DocumentName,
            request.DocumnetType,
            request.DocumentHash);

        await digitalSignRepo.AddAsync(entity, cancellationToken);
        return await _unitOfWork.SaveChangesAsync(cancellationToken) > 0;
    }
}
