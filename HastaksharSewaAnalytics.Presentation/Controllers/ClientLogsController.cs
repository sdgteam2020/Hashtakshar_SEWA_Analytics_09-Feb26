using HastaksharSewaAnalytics.Application.Abstractions.Interfaces.IServices;
using HastaksharSewaAnalytics.Application.Dtos.ClientLogs;
using HastaksharSewaAnalytics.Presentation.Logging;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HastaksharSewaAnalytics.Presentation.Controllers;

[Authorize(Policy = "DeviceOnly")]
public class ClientLogsController : Controller
{
    private readonly ILogger<ClientLogsController> _logger;
    private readonly IClientLogsService _clientLogsRepository;
    public ClientLogsController(ILogger<ClientLogsController> logger, IClientLogsService clientLogsRepository)
    {
        _logger = logger;
        _clientLogsRepository = clientLogsRepository;
    }
    public IActionResult Index()
    {
        return View();
    }

    [Authorize(Policy = "DeviceOnly")]
    [HttpPost("api/ClientLogs/SaveClientLogs")]
    [IgnoreAntiforgeryToken]
    public async Task<IActionResult> SaveClientLogs([FromBody]ClientErrorLogRequest clientErrorLogRequest, CancellationToken cancellationToken)
    {
        try
        {
            if(!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                return BadRequest(new { Errors = errors });
            }
            var result = await _clientLogsRepository.SaveClientLogAsync(clientErrorLogRequest, cancellationToken);
            if (result == true)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }
        catch (Exception ex)
        {
            ErrorLog.LogErrorToFile(ex, "An error occurred while saving client logs.");
            return BadRequest(false);
        }
    }
}
