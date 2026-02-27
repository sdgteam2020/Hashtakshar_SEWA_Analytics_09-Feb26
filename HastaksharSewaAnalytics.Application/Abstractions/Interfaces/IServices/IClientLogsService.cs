using HastaksharSewaAnalytics.Application.Dtos.ClientLogs;

namespace HastaksharSewaAnalytics.Application.Abstractions.Interfaces.IServices;

public interface IClientLogsService
{
    Task<bool> SaveClientLogAsync(
        ClientErrorLogRequest clientErrorLog,
        CancellationToken cancellationToken = default
        );
}
