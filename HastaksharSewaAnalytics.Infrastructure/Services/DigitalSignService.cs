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

    public async Task<List<GetDigitalSignRespone>> GetDigitalSignListAsync(CancellationToken cancellationToken = default)
    {
        var digitalSign = _UoW.Repository<DigitalSignDetail, int>().Query();
        var userData = _UoW.Repository<VaultMaster, int>().Query();
        var data = await (from ds in digitalSign
                          join ud in userData on ds.ValtMasterId equals ud.Id
                          orderby ds.Id descending
                          select new GetDigitalSignRespone
                          (
                              ds.Id,
                              ds.ValtMasterId,
                              ds.DocumentName!,
                              ds.SignDateTime!,
                              ds.IpAddress!
                          )).ToListAsync(cancellationToken);

        return data;
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
