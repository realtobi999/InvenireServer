using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace InvenireServer.Presentation.Controllers;

/// <summary>
/// Provides basic server status endpoints for health and authentication checks.
/// </summary>
[ApiController]
public class ServerController : ControllerBase
{
    /// <summary>
    /// Checks if the server is running and responsive.
    /// </summary>
    /// <returns>Returns HTTP 200 OK if the server is healthy.</returns>
    [HttpGet("/api/server/health-check")]
    public IActionResult HealthCheck()
    {
        return Ok();
    }

    /// <summary>
    /// Verifies that the current user is authenticated.
    /// </summary>
    /// <returns>Returns HTTP 200 OK if the user is authenticated.</returns>
    [Authorize]
    [HttpGet("/api/server/auth-check")]
    public IActionResult AuthCheck()
    {
        return Ok();
    }
}
