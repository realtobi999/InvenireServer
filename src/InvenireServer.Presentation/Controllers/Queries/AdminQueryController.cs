using InvenireServer.Application.Core.Admins.Queries.GetByJwt;
using InvenireServer.Domain.Entities.Common;
using InvenireServer.Infrastructure.Authentication;
using InvenireServer.Presentation.Extensions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InvenireServer.Presentation.Controllers.Queries;

/// <summary>
/// Controller for admin queries.
/// </summary>
[ApiController]
public class AdminQueryController : ControllerBase
{
    private readonly IMediator _mediator;

    public AdminQueryController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Handles the request to get the current admin profile.
    /// </summary>
    /// <returns>Awaitable task returning the response.</returns>
    [Authorize(Roles = Jwt.Roles.ADMIN)]
    [HttpGet("/api/admins/profile")]
    public async Task<IActionResult> GetByJwt()
    {
        return Ok((Application.Dtos.Admins.AdminDto?)await _mediator.Send(new GetByJwtAdminQuery
        {
            Jwt = JwtBuilder.Parse(HttpContext.Request.ParseJwtToken()),
        }));
    }
}
