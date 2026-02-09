using HastaksharSewaAnalytics.Application.Dtos.DigitalSign;

namespace HastaksharSewaAnalytics.Application.Abstractions.Interfaces.IServices;

public interface IDigitalSignService
{
    Task<int> GetDigitalSignCountAsync(CancellationToken cancellationToken = default);
    Task<List<GetDigitalSignRespone>> GetDigitalSignListAsync(CancellationToken cancellationToken = default);
    Task<bool> SaveDigitalSign(SaveDigitalSignRequest saveDigitalSignRequest, CancellationToken cancellationToken = default);
}
