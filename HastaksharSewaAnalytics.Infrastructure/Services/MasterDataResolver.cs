using HastaksharSewaAnalytics.Application.Abstractions.Interfaces.Common;
using HastaksharSewaAnalytics.Domain.Entities;
using HastaksharSewaAnalytics.Domain.Premitives.Enums;
using System.Security.Cryptography;
using System.Text;

namespace HastaksharSewaAnalytics.Infrastructure.Services;

public sealed class MasterDataResolver
{
    private const string HastaksharAppName = "HastaksharSewa";
    private const string UnknownVersion = "UNKNOWN";

    private readonly IUnitOfWork _unitOfWork;

    public MasterDataResolver(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

    public Task<int> GetOrCreateHastaksharVersionAsync(
        string version,
        string? createdBy,
        CancellationToken ct)
        => GetOrCreateApplicationVersionAsync(HastaksharAppName, version, createdBy, ct);

    public async Task<int> GetOrCreateClientAsync(
        string? domainId,
        string? ipAddress,
        string? createdBy,
     CancellationToken ct)
    {
        domainId = domainId?.Trim();
        ipAddress = ipAddress?.Trim();

        if (string.IsNullOrWhiteSpace(domainId) && string.IsNullOrWhiteSpace(ipAddress))
            throw new ArgumentException("Either DomainId or IP Address is required.");

        var clientRepo = _unitOfWork.Repository<ClientMaster, int>();

        // Case 1: Only DomainId OR only IPAddress passed -> Get Client
        if (string.IsNullOrWhiteSpace(ipAddress) || string.IsNullOrWhiteSpace(domainId))
        {
            var clientId = await clientRepo.GetScalarAsync(
                selector: x => x.Id,
                predicate: x =>
                    (!string.IsNullOrWhiteSpace(domainId) && x.DomainId == domainId) ||
                    (!string.IsNullOrWhiteSpace(ipAddress) && x.IPAddress == ipAddress),
                cancellationToken: ct);

            if (clientId > 0)
                return clientId;

            throw new KeyNotFoundException("Client not found.");
        }

        // Case 2: Both DomainId and IPAddress passed -> Create Client
        var existingClientId = await clientRepo.GetScalarAsync(
            selector: x => x.Id,
            predicate: x => x.DomainId == domainId && x.IPAddress == ipAddress,
            cancellationToken: ct);

        if (existingClientId > 0)
            return existingClientId;

        var client = ClientMaster.Create(domainId, ipAddress);
        
        await clientRepo.AddAsync(client, ct);
        await _unitOfWork.SaveChangesAsync(ct);

        return client.Id;
    }

    public async Task<int> GetOrCreateApplicationVersionAsync(
        string appName,
        string? version,
        string? createdBy,
        CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(appName))
            throw new ArgumentException("AppName is required.", nameof(appName));
        var appRepo = _unitOfWork.Repository<ApplicationMaster, int>();
        var appId = await appRepo.GetScalarAsync(
            selector: x => x.Id,
            predicate: x => x.AppName == appName,
            cancellationToken: ct);

        if (appId <= 0)
        {
            var app = ApplicationMaster.Create(appName);
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

        var appVersion = ApplicationVersion.Create(appId, normalizedVersion);
        await versionRepo.AddAsync(appVersion, ct);
        await _unitOfWork.SaveChangesAsync(ct);
        return appVersion.Id;
    }
    
}
