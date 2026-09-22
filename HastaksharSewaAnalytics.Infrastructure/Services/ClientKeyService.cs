using HastaksharSewaAnalytics.Application.Abstractions.Interfaces.Common;
using HastaksharSewaAnalytics.Application.Abstractions.Interfaces.ISecurity;
using HastaksharSewaAnalytics.Application.Abstractions.Interfaces.IServices;
using HastaksharSewaAnalytics.Application.Dtos;
using HastaksharSewaAnalytics.Application.Dtos.Devices;
using HastaksharSewaAnalytics.Domain.Entities;
using System.Security.Claims;

namespace HastaksharSewaAnalytics.Infrastructure.Services;

public sealed class ClientKeyService : IClientKeyService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IClientKeyHasher _deviceKeyHasher;
    private readonly IJwtTokenService _jwtTokenService;
    private readonly MasterDataResolver _masterDataResolver;

    public ClientKeyService(IUnitOfWork unitOfWork, IClientKeyHasher deviceKeyHasher, IJwtTokenService jwtTokenService, MasterDataResolver masterDataResolver)
    {
        _unitOfWork = unitOfWork;
        _deviceKeyHasher = deviceKeyHasher;
        _jwtTokenService = jwtTokenService;
        _masterDataResolver = masterDataResolver;
    }

    public async Task<TokenResponse?> AuthenticateDeviceAsync(ClientKeyRequest request, CancellationToken cancellationToken = default)
    {
        try
        {
            if (request == null ||
                string.IsNullOrWhiteSpace(request.DomainId) ||
                string.IsNullOrWhiteSpace(request.IPAddress) ||
                string.IsNullOrWhiteSpace(request.ClientKey))
                return null;

            var clientId = await _masterDataResolver.GetOrCreateClientAsync(
                request.DomainId.Trim(),
                request.IPAddress.Trim(),
                null,
                cancellationToken);

            var repo = _unitOfWork.Repository<ClientKey, int>();

            var deviceKey = request.ClientKey.Trim();

            int id;


            try
            {
                id = await repo.GetScalarAsync(
                    selector: d => d.Id,
                    predicate: d => d.CreatedByClientId == clientId,
                    cancellationToken: cancellationToken);
            }
            catch
            {
                id = 0;
            }

            ClientKey? clientKey = null;

            if (id == 0)
            {
                var (hash, salt) = _deviceKeyHasher.Hash(deviceKey);

                clientKey = ClientKey.Create(clientId, hash, salt);

                await repo.AddAsync(clientKey, cancellationToken);
                await _unitOfWork.SaveChangesAsync(cancellationToken);
            }
            else
            {
                clientKey = await repo.GetByIdAsync(id, cancellationToken);
                if (clientKey == null) return null;
                if (!clientKey.IsActive) return null;

                var keyOk = _deviceKeyHasher.Verify(deviceKey, clientKey.ClientKeyHash, clientKey.ClientKeySalt);
                if (!keyOk) return null;
            }

            var claims = new List<Claim>
            {
                new("token_type", "client"),
                new("client_id", clientKey.CreatedByClientId.ToString()),
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

    public Task<bool> UnregisterDeviceAsync(string deviceId, CancellationToken cancellationToken = default)
        => throw new NotImplementedException();
}
