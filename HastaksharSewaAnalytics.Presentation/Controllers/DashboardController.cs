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

    //GET: /Dashboard/TotalInstallCount
    [HttpGet("/Dashboard/TotalInstallCount")]
    [Authorize]
    public async Task<IActionResult> TotalInstallCount()
    {
        dynamic totalCount = 0;
        try
        {
            // Query PostgreSQL table
            totalCount = await _dashboardRepository.GetTotalInstallationsAsync();

            // Option 1: return as JSON (for AJAX)
        }
        catch (Exception ex)
        {
            ErrorLog.LogErrorToFile(ex, "An error occurred while fetching total installation count.");
        }
        return Json(new { totalInstallations = totalCount });

    }

    // GET: /Dashboard/TodayUsers
    [HttpGet("/Dashboard/TodayUserCount")]
    [Authorize]
    public async Task<IActionResult> TodayUserCount()
    {
        dynamic count = 0;
        try
        {
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
    public async Task<IActionResult> GetApplications()
    {
        try
        {
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
    public async Task<IActionResult> GetTodayUsers()
    {
        try
        {
            //int a = 0;
            //int b = 1;
            //int c = b / a;
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
    public async Task<IActionResult> GetClientErrorLogsCount()
    {
        dynamic count = 0;
        try
        {
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
    public async Task<IActionResult> GetClientErrorLogsData([FromHeader(Name = "X-Requested-With")] string xrw)
    {
        try
        {
            if (xrw != "XMLHttpRequest")
                return Forbid();
            var data = await _dashboardRepository.GetClientErrorLogsData();
            return Json(data);
        }
        catch (Exception ex)
        {
            ErrorLog.LogErrorToFile(ex, "An error occurred while fetching client error logs data.");
        }
        return Json(new { });
    }

    [HttpGet]
    [Authorize]
    public async Task<IActionResult> GetVaultDataCount()
    {
        dynamic count = 0;
        try
        {
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
    public async Task<IActionResult> GetVaultMasterData()
    {
        try
        {
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
    public async Task<IActionResult> GetDigitalSignCount()
    {
        dynamic count = 0;
        try
        {
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
    public async Task<IActionResult> GetDigitalSignList()
    {
        try
        {
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
