using HastaksharSewaAnalytics.Application.Dtos;
using HastaksharSewaAnalytics.Application.Dtos.Devices;

namespace HastaksharSewaAnalytics.Application.Abstractions.Interfaces.IServices;

public interface IClientKeyService
{    
    Task<bool> UnregisterDeviceAsync(
        string deviceId,
        CancellationToken cancellationToken = default);
    Task<TokenResponse?> AuthenticateDeviceAsync(
        ClientKeyRequest request,
        CancellationToken cancellationToken = default);
}
