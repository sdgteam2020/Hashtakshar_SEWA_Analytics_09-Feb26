using HastaksharSewaAnalytics.Application.Abstractions.Interfaces.Common;
using HastaksharSewaAnalytics.Domain.Entities;
using System.Security.Cryptography;
using System.Text;

namespace HastaksharSewaAnalytics.Infrastructure.Services;

internal sealed class MasterDataResolver
{
    private const string HastaksharAppCode = "HASTAKSHARSEWA";
    private const string HastaksharAppName = "Hastakshar Sewa";
    private const string UnknownVersion = "UNKNOWN";

    private readonly IUnitOfWork _unitOfWork;

    public MasterDataResolver(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

    public Task<int> GetOrCreateHastaksharVersionAsync(
        string version,
        string? createdBy,
        CancellationToken ct)
        => GetOrCreateApplicationVersionAsync(HastaksharAppCode, HastaksharAppName, version, createdBy, ct);

    public Task<int> GetOrCreateApplicationVersionAsync(
        string appName,
        string? version,
        string? createdBy,
        CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(appName))
            throw new ArgumentException("AppName is required.", nameof(appName));

        var appCode = BuildApplicationCode(appName);
        return GetOrCreateApplicationVersionAsync(appCode, appName.Trim(), version, createdBy, ct);
    }

    public async Task<int> GetOrCreateClientAsync(
        string domainId,
        string ipAddress,
        string? externalDeviceId,
        string? createdBy,
        CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(domainId))
            throw new ArgumentException("DomainId is required.", nameof(domainId));

        domainId = domainId.Trim();
        ipAddress = ipAddress.Trim();

        int? devicePk = null;
        if (!string.IsNullOrWhiteSpace(externalDeviceId))
        {
            var deviceRepo = _unitOfWork.Repository<Device, int>();
            var id = await deviceRepo.GetScalarAsync(
                selector: x => x.Id,
                predicate: x => x.DeviceId == externalDeviceId.Trim(),
                cancellationToken: ct);

            if (id > 0)
                devicePk = id;
        }

        var clientRepo = _unitOfWork.Repository<ClientMaster, int>();
        var clientId = await clientRepo.GetScalarAsync(
            selector: x => x.Id,
            predicate: x => x.DomainId == domainId,
            cancellationToken: ct);

        if (clientId > 0)
        {
            var existing = await clientRepo.GetByIdAsync(clientId, ct);
            if (existing != null &&
                (!string.Equals(existing.IPAddress, ipAddress, StringComparison.OrdinalIgnoreCase) ||
                 (devicePk.HasValue && existing.DeviceId != devicePk)))
            {
                existing.UpdateConnection(ipAddress, devicePk, createdBy ?? "System");
                await _unitOfWork.SaveChangesAsync(ct);
            }

            return clientId;
        }

        var client = ClientMaster.Create(domainId, ipAddress, devicePk, createdBy);
        await clientRepo.AddAsync(client, ct);
        await _unitOfWork.SaveChangesAsync(ct);
        return client.Id;
    }

    private async Task<int> GetOrCreateApplicationVersionAsync(
        string appCode,
        string appName,
        string? version,
        string? createdBy,
        CancellationToken ct)
    {
        var appRepo = _unitOfWork.Repository<ApplicationMaster, int>();
        var appId = await appRepo.GetScalarAsync(
            selector: x => x.Id,
            predicate: x => x.AppCode == appCode,
            cancellationToken: ct);

        if (appId <= 0)
        {
            var app = ApplicationMaster.Create(appCode, appName, createdBy);
            await appRepo.AddAsync(app, ct);
            await _unitOfWork.SaveChangesAsync(ct);
            appId = app.Id;
        }

        var normalizedVersion = string.IsNullOrWhiteSpace(version) ? UnknownVersion : version.Trim();
        var versionRepo = _unitOfWork.Repository<ApplicationVersion, int>();
        var versionId = await versionRepo.GetScalarAsync(
            selector: x => x.Id,
            predicate: x => x.ApplicationId == appId && x.Version == normalizedVersion,
            cancellationToken: ct);

        if (versionId > 0)
            return versionId;

        var appVersion = ApplicationVersion.Create(appId, normalizedVersion, createdBy);
        await versionRepo.AddAsync(appVersion, ct);
        await _unitOfWork.SaveChangesAsync(ct);
        return appVersion.Id;
    }

    private static string BuildApplicationCode(string appName)
    {
        var normalized = appName.Trim().ToUpperInvariant();
        var hash = MD5.HashData(Encoding.UTF8.GetBytes(normalized));
        return $"APP_{Convert.ToHexString(hash)[..12]}";
    }
}
