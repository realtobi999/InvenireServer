using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace InvenireServer.Presentation.Controllers;

[ApiController]
public class ServerController : ControllerBase
{
    [HttpGet("/api/server/health-check")]
    public IActionResult HealthCheck()
    {
        return Ok();
    }

    [Authorize]
    [HttpGet("/api/server/auth-check")]
    public IActionResult AuthCheck()
    {
        return Ok();
    }
}