using HastaksharSewaAnalytics.Application.Abstractions.Interfaces.Common;
using HastaksharSewaAnalytics.Application.Abstractions.Interfaces.IServices;
using HastaksharSewaAnalytics.Application.Dtos.ClientLogs;
using HastaksharSewaAnalytics.Domain.Entities;

namespace HastaksharSewaAnalytics.Infrastructure.Services;

public sealed record ClientLogsService : IClientLogsService
{
    private readonly IUnitOfWork _unitOfWork;
    public ClientLogsService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
    }
    public async Task<bool> SaveClientLogAsync(ClientErrorLogRequest clientErrorLogRequest, CancellationToken cancellationToken = default)
    {
        if (clientErrorLogRequest == null)
        {
            throw new ArgumentNullException(nameof(clientErrorLogRequest));
        }
        var log = ClientErrorLog.Create(
            clientErrorLogRequest.AppName,
            clientErrorLogRequest.ErrorMessage,
            clientErrorLogRequest.StackTrace,
            clientErrorLogRequest.IpAddress,
            clientErrorLogRequest.MachineName,
            clientErrorLogRequest.UserName,
            clientErrorLogRequest.OperatingSystem,
            clientErrorLogRequest.Is64Bit,
            clientErrorLogRequest.SystemDirectory,
            clientErrorLogRequest.AppVersion,
            clientErrorLogRequest.Extra
        );
        var repository = _unitOfWork.Repository<ClientErrorLog, int>();
        await repository.AddAsync(log);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return true;
    }
}
