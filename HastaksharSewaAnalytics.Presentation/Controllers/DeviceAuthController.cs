using HastaksharSewaAnalytics.Application.Abstractions.Interfaces.IServices;
using HastaksharSewaAnalytics.Application.Dtos.Devices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HastaksharSewaAnalytics.Presentation.Controllers;

[Route("api/device-auth/")]
[ApiController]
[AllowAnonymous]
public sealed class DeviceAuthController : ControllerBase
{
    private readonly IDeviceService _deviceService;

    public DeviceAuthController(IDeviceService deviceService)
    {
        _deviceService = deviceService;
    }

    // POST: /api/device-auth/token
    [HttpPost("token"), AllowAnonymous]
    public async Task<IActionResult> Token([FromBody] DeviceRequest request, CancellationToken ct)
    {
        try
        {
            var token = await _deviceService.AuthenticateDeviceAsync(request, ct);
            if (token == null) return Unauthorized(new { message = "Invalid device credentials." });
            return Ok(token);
        }
        catch (Exception ex)
        {
            // Log the exception (not implemented here)
            return StatusCode(500, new { message = "An error occurred while processing the request.", details = ex.Message });
        }
    }
}