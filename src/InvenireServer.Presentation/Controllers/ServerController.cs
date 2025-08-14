using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using InvenireServer.Presentation.Extensions;
using InvenireServer.Infrastructure.Authentication;

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

    [Authorize]
    [HttpGet("/api/server/auth/role")]
    public IActionResult GetRole()
    {
        return Ok(JwtBuilder.Parse(HttpContext.Request.ParseJwtToken()).GetRole());
    }
}