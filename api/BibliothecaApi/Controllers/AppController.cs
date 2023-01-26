using BibliothecaApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BookStoreApi.Controllers;

[ApiController]
[Authorize(Roles = "Admin")]
[Route("api/[controller]")]
public class AppController : ControllerBase
{
    private readonly AppService _appService;

    public AppController(AppService appService) =>
        _appService = appService;

    [HttpPost("BackupDatabase")]
    public async Task<IActionResult> BackupDatabase()
    {
        await _appService.BackupDatabase();
        return Ok();
    }

    [HttpPost("RestoreDatabase")]
    public async Task<IActionResult> RestoreDatabase()
    {
        await _appService.RestoreDatabase();
        return Ok();
    }

}