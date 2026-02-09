using HastaksharSewaAnalytics.Application.Abstractions.Interfaces.IServices;
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
        // Query PostgreSQL table
        var totalCount = await _dashboardRepository.GetTotalInstallationsAsync();

        // Option 1: return as JSON (for AJAX)
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
        catch (Exception) { }

        return Json(new { todayUsers = count });
    }

    [HttpGet]
    [Authorize]
    public async Task<IActionResult> GetApplications()
    {        
        var data = await _dashboardRepository.GetHastaksharSewaInstallationsQuery();
        return Json(data);
    }

    [HttpGet]
    [Authorize]
    public async Task<IActionResult> GetTodayUsers()
    {
        var data = await _dashboardRepository.GetHastaksharSewaDailyRunQuery();
        return Json(data);
    }
    [HttpGet]
    [Authorize]
    public async Task<IActionResult> GetClientErrorLogsCount()
    {
        var count = await _dashboardRepository.GetClientErrorLogsCounts();
        return Json(new { clientErrorLogsCount = count });
    }
    [HttpGet]
    [Authorize]
    public async Task<IActionResult> GetClientErrorLogsData([FromHeader(Name = "X-Requested-With")] string xrw)
    {
        if (xrw != "XMLHttpRequest")
            return Forbid();
        var data = await _dashboardRepository.GetClientErrorLogsData();
        return Json(data);
    }

    [HttpGet]
    [Authorize]
    public async Task<IActionResult> GetVaultDataCount()
    {
        var count = await _dashboardRepository.GetVaultDataCount();
        return Json(new { vaultDataCount = count });
    }

    [HttpGet]
    [Authorize]
    public async Task<IActionResult> GetVaultMasterData()
    {
        var data = await _dashboardRepository.GetVaultMasterData();
        return Json(data);
    }
    [HttpGet]
    [Authorize]
    public async Task<IActionResult> GetDigitalSignCount()
    {
        var count = await _digitalSignService.GetDigitalSignCountAsync();
        return Json(new { digitalSignCount = count });
    }
    [HttpGet]
    [Authorize]
    public async Task<IActionResult> GetDigitalSignList()
    {
        var data = await _digitalSignService.GetDigitalSignListAsync();
        return Json(data);
    }
}
