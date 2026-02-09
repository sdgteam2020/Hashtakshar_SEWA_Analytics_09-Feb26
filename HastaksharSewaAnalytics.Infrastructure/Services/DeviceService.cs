using HastaksharSewaAnalytics.Application.Abstractions.Interfaces.Common;
using HastaksharSewaAnalytics.Application.Abstractions.Interfaces.ISecurity;
using HastaksharSewaAnalytics.Application.Abstractions.Interfaces.IServices;
using HastaksharSewaAnalytics.Application.Dtos;
using HastaksharSewaAnalytics.Application.Dtos.Devices;
using HastaksharSewaAnalytics.Domain.Entities;
using System.Security.Claims;

namespace HastaksharSewaAnalytics.Infrastructure.Services;

public sealed class DeviceService : IDeviceService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IDeviceKeyHasher _deviceKeyHasher;
    private readonly IJwtTokenService _jwtTokenService;

    public DeviceService(IUnitOfWork unitOfWork, IDeviceKeyHasher deviceKeyHasher, IJwtTokenService jwtTokenService)
    {
        _unitOfWork = unitOfWork;
        _deviceKeyHasher = deviceKeyHasher;
        _jwtTokenService = jwtTokenService;
    }

    public async Task<TokenResponse?> AuthenticateDeviceAsync(DeviceRequest request, CancellationToken cancellationToken = default)
    {
        try
        {
            if (request == null ||
                string.IsNullOrWhiteSpace(request.DeviceId) ||
                string.IsNullOrWhiteSpace(request.DeviceKey))
                return null;

            var repo = _unitOfWork.Repository<Device, Guid>();

            var deviceId = request.DeviceId.Trim();
            var deviceKey = request.DeviceKey.Trim();

            Guid id;

            // Try to get existing device Id (if none, GetScalarAsync might throw)
            try
            {
                id = await repo.GetScalarAsync(
                    selector: d => d.Id,
                    predicate: d => d.DeviceId == deviceId,
                    cancellationToken: cancellationToken);
            }
            catch
            {
                id = Guid.Empty;
            }

            Device? device = null;

            // Auto-register if not found
            if (id == Guid.Empty)
            {
                var (hash, salt) = _deviceKeyHasher.Hash(deviceKey);

                device = Device.Create(deviceId, hash, salt);

                await repo.AddAsync(device, cancellationToken);
                await _unitOfWork.SaveChangesAsync(cancellationToken);
            }
            else
            {
                device = await repo.GetByIdAsync(id, cancellationToken);
                if (device == null) return null;
                if (!device.IsActive) return null;

                var keyOk = _deviceKeyHasher.Verify(deviceKey, device.DeviceKeyHash, device.DeviceKeySalt);
                if (!keyOk) return null;
            }

            // ✅ Issue DEVICE JWT
            var claims = new List<Claim>
            {
                new("token_type", "device"),
                new("device_id", device.DeviceId),
                new("scope", "api.read api.write")
            };

            var expiresUtc = DateTime.UtcNow.AddMinutes(_jwtTokenService.AccessTokenMinutes);
            var token = _jwtTokenService.CreateToken(claims, expiresUtc);

            return new TokenResponse
            {
                AccessToken = token,
                ExpiresInSeconds = _jwtTokenService.AccessTokenMinutes * 60,
                TokenType = "Bearer"
            };
        }
        catch
        {
            return null;
        }
    }

    public async Task<bool> IsValidDeviceAsync(DeviceRequest request, CancellationToken cancellationToken = default)
    {
        try
        {
            if (request == null || string.IsNullOrWhiteSpace(request.DeviceId))
                return false;

            var repo = _unitOfWork.Repository<Device, Guid>();

            var isActive = await repo.GetScalarAsync(
                selector: d => d.IsActive,
                predicate: d => d.DeviceId == request.DeviceId,
                cancellationToken: cancellationToken);

            return isActive;
        }
        catch
        {
            return false;
        }
    }

    public async Task<bool> RegisterDeviceAsync(DeviceRequest request, CancellationToken cancellationToken = default)
    {
        try
        {
            if (request == null ||
                string.IsNullOrWhiteSpace(request.DeviceId) ||
                string.IsNullOrWhiteSpace(request.DeviceKey))
                return false;

            var repo = _unitOfWork.Repository<Device, Guid>();

            // Prevent duplicates
            var existing = await repo.CountAsync(
                predicate: d => d.DeviceId == request.DeviceId,
                ct: cancellationToken) > 0;

            if (existing) return false;

            var (hashedDeviceKey, deviceKeySalt) = _deviceKeyHasher.Hash(request.DeviceKey);

            var device = Device.Create(
                request.DeviceId.Trim(),
                hashedDeviceKey,
                deviceKeySalt
            );

            await repo.AddAsync(device, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return true;
        }
        catch
        {
            return false;
        }
    }

    public Task<bool> UnregisterDeviceAsync(string deviceId, CancellationToken cancellationToken = default)
        => throw new NotImplementedException();
}
