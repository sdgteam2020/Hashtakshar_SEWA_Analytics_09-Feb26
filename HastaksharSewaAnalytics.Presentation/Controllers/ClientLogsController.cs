using HastaksharSewaAnalytics.Application.Abstractions.Interfaces.IServices;
using HastaksharSewaAnalytics.Application.Dtos.ClientLogs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HastaksharSewaAnalytics.Presentation.Controllers;

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
    //[AllowAnonymous]
    [HttpPost("api/ClientLogs/SaveClientLogs")]
    public async Task<IActionResult> SaveClientLogs([FromBody]ClientErrorLogRequest clientErrorLogRequest, CancellationToken cancellationToken)
    {
        var result = await _clientLogsRepository.SaveClientLogAsync(clientErrorLogRequest, cancellationToken);
        if (result == true)
        {
            return Ok(result);
        }
        return BadRequest(result);
    }
}
