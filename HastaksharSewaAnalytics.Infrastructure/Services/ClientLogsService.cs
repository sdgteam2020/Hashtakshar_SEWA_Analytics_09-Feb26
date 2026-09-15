using HastaksharSewaAnalytics.Application.Abstractions.Interfaces.Common;
using HastaksharSewaAnalytics.Application.Abstractions.Interfaces.IServices;
using HastaksharSewaAnalytics.Application.Dtos.ClientLogs;
using HastaksharSewaAnalytics.Domain.Entities;
using System.Security.Cryptography;
using System.Text;

namespace HastaksharSewaAnalytics.Infrastructure.Services;

public sealed record ClientLogsService : IClientLogsService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly MasterDataResolver _masterDataResolver;

    public ClientLogsService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _masterDataResolver = new MasterDataResolver(_unitOfWork);
    }

    public async Task<bool> SaveClientLogAsync(
        ClientErrorLogRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        var domainId = string.IsNullOrWhiteSpace(request.DomainId)
            ? BuildLegacyDomainKey(request.MachineName)
            : request.DomainId.Trim();

        var clientId = await _masterDataResolver.GetOrCreateClientAsync(
            domainId,
            request.IpAddress,
            request.DeviceId,
            createdBy: null,
            ct: cancellationToken);

        var applicationVersionId = await _masterDataResolver.GetOrCreateApplicationVersionAsync(
            request.AppName,
            request.AppVersion,
            createdBy: null,
            ct: cancellationToken);

        var repository = _unitOfWork.Repository<ClientErrorLog, int>();

        if (request.RequestId.HasValue)
        {
            var duplicate = await repository.CountAsync(
                x => x.RequestId == request.RequestId,
                cancellationToken);

            if (duplicate > 0)
                return true;
        }

        var log = ClientErrorLog.Create(
            clientId,
            applicationVersionId,
            request.ErrorMessage,
            request.StackTrace,
            request.MachineName,
            request.UserName,
            request.OperatingSystem,
            request.Is64Bit,
            request.SystemDirectory,
            request.Extra,
            request.RequestId);

        await repository.AddAsync(log, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return true;
    }

    private static string BuildLegacyDomainKey(string machineName)
    {
        if (!string.IsNullOrWhiteSpace(machineName) && machineName.Trim().Length <= 64)
            return machineName.Trim();

        var normalized = string.IsNullOrWhiteSpace(machineName)
            ? "UNKNOWN-MACHINE"
            : machineName.Trim().ToUpperInvariant();
        var hash = MD5.HashData(Encoding.UTF8.GetBytes(normalized));
        return $"MACHINE_{Convert.ToHexString(hash)[..12]}";
    }
}
