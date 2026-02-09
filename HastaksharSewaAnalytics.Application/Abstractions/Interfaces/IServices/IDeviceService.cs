using HastaksharSewaAnalytics.Application.Dtos;
using HastaksharSewaAnalytics.Application.Dtos.Devices;

namespace HastaksharSewaAnalytics.Application.Abstractions.Interfaces.IServices;

public interface IDeviceService
{
    Task<bool> IsValidDeviceAsync(
        DeviceRequest request,
        CancellationToken cancellationToken = default);
    Task<bool> RegisterDeviceAsync(
        DeviceRequest request,
        CancellationToken cancellationToken = default);
    Task<bool> UnregisterDeviceAsync(
        string deviceId,
        CancellationToken cancellationToken = default);
    Task<TokenResponse?> AuthenticateDeviceAsync(
        DeviceRequest request,
        CancellationToken cancellationToken = default);
}
