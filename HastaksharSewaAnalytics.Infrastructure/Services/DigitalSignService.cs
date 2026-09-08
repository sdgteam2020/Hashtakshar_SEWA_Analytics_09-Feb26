using HastaksharSewaAnalytics.Application.Abstractions.Interfaces.Common;
using HastaksharSewaAnalytics.Application.Abstractions.Interfaces.IServices;
using HastaksharSewaAnalytics.Application.Dtos.DigitalSign;
using HastaksharSewaAnalytics.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace HastaksharSewaAnalytics.Infrastructure.Services;

public sealed record DigitalSignService : IDigitalSignService
{
    private readonly IUnitOfWork _UoW;

    public DigitalSignService(IUnitOfWork uoW)
    {
        _UoW = uoW;
    }

    public async Task<int> GetDigitalSignCountAsync(CancellationToken cancellationToken = default)
    {
        return await _UoW.Repository<DigitalSignDetail, int>()
            .CountAsync(null, cancellationToken);
    }

    public async Task<(List<GetDigitalSignRespone> Data, int TotalCount, int FilteredCount)> GetDigitalSignListAsync(
        int start,
        int length,
        string? searchValue,
        CancellationToken cancellationToken = default)
    {
        start = Math.Max(start, 0);
        length = Math.Clamp(length, 1, 100);

        var digitalSign = _UoW.Repository<DigitalSignDetail, int>().Query();
        var userData = _UoW.Repository<VaultMaster, int>().Query();

        var query = from ds in digitalSign
                    join ud in userData on ds.ValtMasterId equals ud.Id
                    select new
                    {
                        ds.Id,
                        ds.ValtMasterId,
                        ds.DocumentName,
                        ds.SignDateTime,
                        ds.IpAddress
                    };

        var totalCount = await query.CountAsync(cancellationToken);

        if (!string.IsNullOrWhiteSpace(searchValue))
        {
            var search = searchValue.Trim();
            var pattern = $"%{search}%";

            if (int.TryParse(search, out var userPublicDataId))
            {
                query = query.Where(x =>
                    x.ValtMasterId == userPublicDataId ||
                    (x.DocumentName != null && EF.Functions.ILike(x.DocumentName, pattern)) ||
                    (x.SignDateTime != null && EF.Functions.ILike(x.SignDateTime, pattern)) ||
                    (x.IpAddress != null && EF.Functions.ILike(x.IpAddress, pattern)));
            }
            else
            {
                query = query.Where(x =>
                    (x.DocumentName != null && EF.Functions.ILike(x.DocumentName, pattern)) ||
                    (x.SignDateTime != null && EF.Functions.ILike(x.SignDateTime, pattern)) ||
                    (x.IpAddress != null && EF.Functions.ILike(x.IpAddress, pattern)));
            }
        }

        var filteredCount = await query.CountAsync(cancellationToken);

        var rows = await query
            .OrderByDescending(x => x.Id)
            .Skip(start)
            .Take(length)
            .ToListAsync(cancellationToken);

        var data = rows.Select(x => new GetDigitalSignRespone(
            x.Id,
            x.ValtMasterId,
            x.DocumentName!,
            x.SignDateTime!,
            x.IpAddress!
        )).ToList();

        return (data, totalCount, filteredCount);
    }

    public async Task<bool> SaveDigitalSign(SaveDigitalSignRequest request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        var userDataRepo = _UoW.Repository<VaultMaster, int>();
        var digitalSignRepo = _UoW.Repository<DigitalSignDetail, int>();

        try
        {
            int publicUserDataId = await userDataRepo.GetScalarAsync(
                selector: s => s.Id,
                predicate: p => p.SerialNo == request.SerialNo
            );

            if (publicUserDataId <= 0)
            {
                var userEntity = VaultMaster.Create(
                    request.PublicKey,
                    request.SerialNo,
                    request.TokenValid,
                    request.ValidFrom,
                    request.ValidTo
                );

                await userDataRepo.AddAsync(userEntity, cancellationToken);
                await _UoW.SaveChangesAsync(cancellationToken);

                publicUserDataId = userEntity.Id;
            }

            var entity = DigitalSignDetail.Create(
                publicUserDataId,
                request.SignedDateTime,
                request.OriginForSign,
                request.RefererForSign,
                request.IpAddress,
                request.DocumentName,
                request.DocumnetType
            );

            await digitalSignRepo.AddAsync(entity, cancellationToken);

            int rows = await _UoW.SaveChangesAsync(cancellationToken);
            return rows > 0;
        }
        catch
        {
            throw;
        }
    }
}
