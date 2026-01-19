using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using InvenireServer.Presentation.Extensions;
using InvenireServer.Infrastructure.Authentication;

namespace InvenireServer.Presentation.Controllers;

/// <summary>
/// Controller for server utility endpoints.
/// </summary>
[ApiController]
public class ServerController : ControllerBase
{
    /// <summary>
    /// Handles the request to log out.
    /// </summary>
    /// <returns>Response.</returns>
    [HttpPost("/api/logout")]
    public IActionResult Logout()
    {
        Response.Cookies.Append("JWT", "", new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.None,
            Expires = DateTimeOffset.UtcNow.AddDays(-1)
        });

        return Ok();
    }

    /// <summary>
    /// Handles the request to check server health.
    /// </summary>
    /// <returns>Response.</returns>
    [HttpGet("/api/server/health-check")]
    public IActionResult HealthCheck()
    {
        return Ok();
    }

    /// <summary>
    /// Handles the request to check authentication.
    /// </summary>
    /// <returns>Response.</returns>
    [Authorize]
    [HttpGet("/api/server/auth-check")]
    public IActionResult AuthCheck()
    {
        return Ok();
    }

    /// <summary>
    /// Handles the request to get the current user role.
    /// </summary>
    /// <returns>Response.</returns>
    [Authorize]
    [HttpGet("/api/server/auth/role")]
    public IActionResult GetRole()
    {
        return Ok(JwtBuilder.Parse(HttpContext.Request.ParseJwtToken()).GetRole());
    }
}
