using HastaksharSewaAnalytics.Application.Abstractions.Interfaces.IServices;
using HastaksharSewaAnalytics.Application.Dtos.Transaction;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HastaksharSewaAnalytics.Presentation.Controllers;

[ApiController]
public class TransactionController : Controller
{
    private readonly ITransactionService _transactionRepository;

    public TransactionController(ITransactionService transactionRepository)
    {
        _transactionRepository = transactionRepository;
    }

    [HttpPost]
    [Authorize(Policy = "DeviceOnly")]
    [Route("api/transaction/SaveUserData")]
    public async Task<IActionResult> SaveVaultMasterData(SaveUserPublicDataRequest obj)
    {
        var result = await _transactionRepository.SaveVaultMasterData(obj);
        if (result == true)
        {
            return Ok(result);
        }
        return BadRequest(result);
    }

    [Authorize(Policy = "DeviceOnly")]
    [HttpPost("api/transaction/SaveInstallationAsync")]
    public async Task<IActionResult> SaveInstallationAsync(SaveInstallationRquest obj)
    {
        var result = await _transactionRepository.SaveInstallationAsync(obj);
        if (result == true)
        {
            return Ok(result);
        }
        return BadRequest(result);
    }

    [Authorize(Policy = "DeviceOnly")]
    [HttpPost("api/transaction/SaveDailyRunAsync")]
    public async Task<IActionResult> SaveDailyRunAsync(SaveDailyRunRequest obj)
    {
        var result = await _transactionRepository.SaveDailyRunAsync(obj);
        if (result == true)
        {
            return Ok(result);
        }
        return BadRequest(result);
    }

    [Authorize(Policy = "DeviceOnly")]
    [HttpPost("api/transaction/search")]
    public async Task<IActionResult> Search([FromBody] VaultSearchRequest req, CancellationToken ct)
    {
        // your WPF is sending ArmyNo = query
        var term = (req?.ArmyNo ?? req?.Term ?? req?.Name ?? "").Trim();

        if (term.Length < 2)
            return Ok(new List<XmlDataForPublicKeyResponse>());

        var data = await _transactionRepository.SearchVaultMastersBySerialAsync(term, ct);
        return Ok(data);
    }


}
