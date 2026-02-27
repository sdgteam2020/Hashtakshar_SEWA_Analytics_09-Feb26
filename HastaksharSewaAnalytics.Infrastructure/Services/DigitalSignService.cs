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
                          orderby ds.SignDateTime descending
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

    public async Task<bool> SaveDigitalSign(SaveDigitalSignRequest saveDigitalSignRequest, CancellationToken cancellationToken = default)
    {
        if (saveDigitalSignRequest == null) throw new ArgumentNullException(nameof(saveDigitalSignRequest));
        try
        {
            var userDataRepo = _UoW.Repository<VaultMaster, int>();
            var digitalSignRepo = _UoW.Repository<DigitalSignDetail, int>();
            int existingCount = await userDataRepo
                                            .CountAsync(
                                                predicate: p => p.SerialNo
                                                .Equals(saveDigitalSignRequest.SerialNo),
                                                cancellationToken);
            int PublicUserDataId = 0;
            if (existingCount <= 0)
            {
                var userEntity = VaultMaster.Create(
                                 saveDigitalSignRequest.PublicKey,
                                 saveDigitalSignRequest.SerialNo,
                                 saveDigitalSignRequest.TokenValid,
                                 saveDigitalSignRequest.ValidFrom,
                                 saveDigitalSignRequest.ValidTo
                                 );

                await userDataRepo.AddAsync(userEntity, cancellationToken);
                PublicUserDataId = userEntity.Id;
            }
            else
            {
                PublicUserDataId = await userDataRepo
                    .GetScalarAsync(
                    selector: s => s.Id,
                    predicate: p => p.SerialNo
                    .Equals(saveDigitalSignRequest.SerialNo)
                    );
            }

            var entity = DigitalSignDetail.Create(
                         PublicUserDataId,
                         saveDigitalSignRequest.SignedDateTime,
                         saveDigitalSignRequest.OriginForSign,
                         saveDigitalSignRequest.RefererForSign,
                         saveDigitalSignRequest.IpAddress,
                         saveDigitalSignRequest.DocumentName,
                         saveDigitalSignRequest.DocumnetType
                         );

            await digitalSignRepo.AddAsync(entity, cancellationToken);
            int isSave = await _UoW.SaveChangesAsync(cancellationToken);
            return isSave > 0;
        }
        catch (Exception)
        { 
            throw;
        }
    }


}
