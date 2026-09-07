using HastaksharSewaAnalytics.Application.Abstractions.Interfaces.IServices;
using HastaksharSewaAnalytics.Presentation.Logging;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HastaksharSewaAnalytics.Presentation.Controllers;

[Authorize]
public sealed class DashboardController : Controller
{
    private readonly IDashboardService _dashboardRepository;
    private readonly IDigitalSignService _digitalSignService;
    public DashboardController(IDashboardService dashboardRepository, IDigitalSignService digitalSignService)
    {
        _dashboardRepository = dashboardRepository;
        _digitalSignService = digitalSignService;
    }
    public IActionResult Dashboard() => View();

    [HttpGet("/Dashboard/TotalInstallCount")]
    [Authorize]
    public async Task<IActionResult> TotalInstallCount([FromHeader(Name = "X-Requested-With")] string xrw)
    {
        dynamic totalCount = 0;
        try
        {
            if (xrw != "XMLHttpRequest")
                return Forbid();
            totalCount = await _dashboardRepository.GetTotalInstallationsAsync();
        }
        catch (Exception ex)
        {
            ErrorLog.LogErrorToFile(ex, "An error occurred while fetching total installation count.");
        }
        return Json(new { totalInstallations = totalCount });

    }

    [HttpGet("/Dashboard/TodayUserCount")]
    [Authorize]
    public async Task<IActionResult> TodayUserCount([FromHeader(Name = "X-Requested-With")] string xrw)
    {
        dynamic count = 0;
        try
        {
            if (xrw != "XMLHttpRequest")
                return Forbid();
            count = await _dashboardRepository.GetTodayUserCount();
        }
        catch (Exception ex)
        {
            ErrorLog.LogErrorToFile(ex, "An error occurred while fetching today's user count.");
        }

        return Json(new { todayUsers = count });
    }

    [HttpGet]
    [Authorize]
    public async Task<IActionResult> GetApplications([FromHeader(Name = "X-Requested-With")] string xrw)
    {
        try
        {
            if (xrw != "XMLHttpRequest")
                return Forbid();
            var data = await _dashboardRepository.GetHastaksharSewaInstallationsQuery();
            return Json(data);
        }
        catch (Exception ex)
        {
            ErrorLog.LogErrorToFile(ex, "An error occurred while fetching applications.");
        }
        return Json(new { });
    }

    [HttpGet]
    [Authorize]
    public async Task<IActionResult> GetTodayUsers([FromHeader(Name = "X-Requested-With")] string xrw)
    {
        try
        {
            if (xrw != "XMLHttpRequest")
                return Forbid();
            var data = await _dashboardRepository.GetHastaksharSewaDailyRunQuery();
            return Json(data);
        }
        catch (Exception ex)
        {
            ErrorLog.LogErrorToFile(ex, "An error occurred while fetching today's users.");
        }
        return Json(new { });
    }
    [HttpGet]
    [Authorize]
    public async Task<IActionResult> GetClientErrorLogsCount([FromHeader(Name = "X-Requested-With")] string xrw)
    {
        dynamic count = 0;
        try
        {
            if (xrw != "XMLHttpRequest")
                return Forbid();
            count = await _dashboardRepository.GetClientErrorLogsCounts();
            return Json(new { clientErrorLogsCount = count });
        }
        catch (Exception ex)
        {
            ErrorLog.LogErrorToFile(ex, "An error occurred while fetching client error logs count.");
        }
        return Json(new { clientErrorLogsCount = count });
    }
    [HttpGet]
    [Authorize]
    public async Task<IActionResult> GetClientErrorLogsData(
     [FromHeader(Name = "X-Requested-With")] string xrw,
     int draw = 1,
     int start = 0,
     int length = 10,
     string? searchValue = null,
     CancellationToken cancellationToken = default)
    {
        try
        {
            if (xrw != "XMLHttpRequest")
                return Forbid();

            start = Math.Max(start, 0);
            length = Math.Clamp(length, 1, 100);

            var result = await _dashboardRepository.GetClientErrorLogsData(
                start,
                length,
                searchValue,
                cancellationToken);

            return Json(new
            {
                draw,
                recordsTotal = result.TotalCount,
                recordsFiltered = result.FilteredCount,
                data = result.Data
            });
        }
        catch (Exception ex)
        {
            ErrorLog.LogErrorToFile(
                ex,
                "An error occurred while fetching client error logs data.");

            return Json(new
            {
                draw,
                recordsTotal = 0,
                recordsFiltered = 0,
                data = Array.Empty<object>()
            });
        }
    }

    [HttpGet]
    [Authorize]
    public async Task<IActionResult> GetVaultDataCount([FromHeader(Name = "X-Requested-With")] string xrw)
    {
        dynamic count = 0;
        try
        {
            if (xrw != "XMLHttpRequest")
                return Forbid();
            count = await _dashboardRepository.GetVaultDataCount();
            return Json(new { vaultDataCount = count });
        }
        catch (Exception ex)
        {
            ErrorLog.LogErrorToFile(ex, "An error occurred while fetching vault data count.");
        }
        return Json(new { vaultDataCount = count });
    }

    [HttpGet]
    [Authorize]
    public async Task<IActionResult> GetVaultMasterData([FromHeader(Name = "X-Requested-With")] string xrw)
    {
        try
        {
            if (xrw != "XMLHttpRequest")
                return Forbid();
            var data = await _dashboardRepository.GetVaultMasterData();
            return Json(data);
        }
        catch (Exception ex)
        {
            ErrorLog.LogErrorToFile(ex, "An error occurred while fetching vault master data.");
        }
        return Json(new { });
    }
    [HttpGet]
    [Authorize]
    public async Task<IActionResult> GetDigitalSignCount([FromHeader(Name = "X-Requested-With")] string xrw)
    {
        dynamic count = 0;
        try
        {
            if (xrw != "XMLHttpRequest")
                return Forbid();
            count = await _digitalSignService.GetDigitalSignCountAsync();
            return Json(new { digitalSignCount = count });
        }
        catch (Exception ex)
        {
            ErrorLog.LogErrorToFile(ex, "An error occurred while fetching digital sign count.");
            return Json(new { digitalSignCount = count });
        }
    }
    [HttpGet]
    [Authorize]
    public async Task<IActionResult> GetDigitalSignList([FromHeader(Name = "X-Requested-With")] string xrw)
    {
        try
        {
            if (xrw != "XMLHttpRequest")
                return Forbid();
            var data = await _digitalSignService.GetDigitalSignListAsync();
            return Json(data);
        }
        catch (Exception ex)
        {
            ErrorLog.LogErrorToFile(ex, "An error occurred while fetching digital sign list.");
        }
        return Json(new { });
    }
}
