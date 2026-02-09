using HastaksharSewaAnalytics.Application.Abstractions.Interfaces.IServices;
using HastaksharSewaAnalytics.Application.Dtos.DigitalSign;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HastaksharSewaAnalytics.Presentation.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DigitalSignController : Controller
{
    private readonly ILogger<DigitalSignController> _logger;
    private readonly IDigitalSignService _digitalSignService;

    public DigitalSignController(ILogger<DigitalSignController> logger, IDigitalSignService digitalSignService)
    {
        _logger = logger;
        _digitalSignService = digitalSignService;
    }

    [Authorize(Policy = "DeviceOnly")]
    [HttpPost("SaveDigitalSign")]
    public async Task<IActionResult> SaveDigitalSign([FromBody] SaveDigitalSignRequest saveDigitalSignRequest, CancellationToken cancellationToken)
    {
        var result = await _digitalSignService.SaveDigitalSign(saveDigitalSignRequest, cancellationToken);
        if (result == true)
        {
            return Ok(result);
        }
        return BadRequest(result);
    }
}
