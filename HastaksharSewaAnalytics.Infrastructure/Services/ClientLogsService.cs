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

        var clientId = await _masterDataResolver.GetOrCreateClientAsync(
            request.MachineName,
            request.IpAddress,
            null,
            ct: cancellationToken);

        var applicationVersionId = await _masterDataResolver.GetOrCreateApplicationVersionAsync(
            request.AppName,
            request.AppVersion,
            createdBy: null,
            ct: cancellationToken);

        

        var repository = _unitOfWork.Repository<ClientErrorLog, int>();

        var log = ClientErrorLog.Create(
            clientId,
            applicationVersionId,
            request.ErrorMessage
          );

        await repository.AddAsync(log, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return true;
    }
}
