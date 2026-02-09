using HastaksharSewaAnalytics.Application.Dtos.ClientLogs;
using HastaksharSewaAnalytics.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace HastaksharSewaAnalytics.Application.Abstractions.Interfaces.IServices;

public interface IClientLogsService
{
    Task<bool> SaveClientLogAsync(
        ClientErrorLogRequest clientErrorLog,
        CancellationToken cancellationToken = default
        );
}
