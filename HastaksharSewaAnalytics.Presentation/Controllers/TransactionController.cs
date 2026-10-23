using HastaksharSewaAnalytics.Application.Abstractions.Interfaces.IServices;
using HastaksharSewaAnalytics.Application.Dtos.Transaction;
using HastaksharSewaAnalytics.Presentation.Logging;
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
    [IgnoreAntiforgeryToken]
    public async Task<IActionResult> SaveVaultMasterData(SaveUserPublicDataRequest obj)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                return BadRequest(new { Errors = errors });
            }
            var result = await _transactionRepository.SaveVaultMasterData(obj);
            if (result == true)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in SaveVaultMasterData: {ex.Message}");
            ErrorLog.LogErrorToFile(ex, $"Error in SaveVaultMasterData for SerialNo: {obj.SerialNo}");
            return BadRequest(false);
        }
    }

    [Authorize(Policy = "DeviceOnly")]
    [HttpPost("api/transaction/SaveInstallationAsync")]
    [IgnoreAntiforgeryToken]
    public async Task<IActionResult> SaveInstallationAsync(SaveInstallationRquest obj)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                return BadRequest(new { Errors = errors });
            }
            var result = await _transactionRepository.SaveInstallationAsync(obj);
            if (result == true)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in SaveInstallationAsync: {ex.Message}");
            ErrorLog.LogErrorToFile(ex, $"Error in SaveInstallationAsync for DomainId: {obj.DomainId}");
            return BadRequest(false);
        }
    }

    [Authorize(Policy = "DeviceOnly")]
    [HttpPost("api/transaction/SaveDailyRunAsync")]
    [IgnoreAntiforgeryToken]
    public async Task<IActionResult> SaveDailyRunAsync(SaveDailyRunRequest obj)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                return BadRequest(new { Errors = errors });
            }
            var result = await _transactionRepository.SaveDailyRunAsync(obj);
            if (result == true)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in SaveDailyRunAsync: {ex.Message}");
            ErrorLog.LogErrorToFile(ex, $"Error in SaveDailyRunAsync for DomainId: {obj.DomainId}");
            return BadRequest(false);
        }
    }
    [Authorize(Policy = "DeviceOnly")]
    [HttpPost("api/transaction/search")]
    [IgnoreAntiforgeryToken]
    public async Task<IActionResult> Search([FromBody] VaultSearchRequest req, CancellationToken ct)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                return BadRequest(new { Errors = errors });
            }
            var term = (req?.ArmyNo ?? req?.Term ?? req?.Name ?? "").Trim();

            if (term.Length < 2)
                return Ok(new List<XmlDataForPublicKeyResponse>());

            var data = await _transactionRepository.SearchVaultMastersBySerialAsync(term, ct);
            return Ok(data);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in Search: {ex.Message}");
            ErrorLog.LogErrorToFile(ex, $"Error in Search for Term: {req?.ArmyNo ?? req?.Term ?? req?.Name}");
            return BadRequest(new List<XmlDataForPublicKeyResponse>());
        }
    }

}
