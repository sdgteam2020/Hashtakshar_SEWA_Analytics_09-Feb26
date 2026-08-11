using HastaksharSewaAnalytics.Application.Dtos.Transaction;

namespace HastaksharSewaAnalytics.Application.Abstractions.Interfaces.IServices;

public interface ITransactionService
{
    Task<bool?> SaveVaultMasterData(
        SaveUserPublicDataRequest userPublicDataRequest,
        CancellationToken cancellationToken = default
        );
    Task<bool> SaveDailyRunAsync(
        SaveDailyRunRequest saveDailyRunRequest,
        CancellationToken ct = default
        );
    Task<bool> SaveInstallationAsync(
        SaveInstallationRquest saveInstallationRquest,
        CancellationToken ct = default
        );
    Task<List<XmlDataForPublicKeyResponse>> SearchVaultMastersBySerialAsync(
        string term, 
        CancellationToken ct
        );
}
