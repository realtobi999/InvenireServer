using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using InvenireServer.Domain.Entities.Common;
using InvenireServer.Presentation.Extensions;
using InvenireServer.Infrastructure.Authentication;
using InvenireServer.Domain.Entities.Common.Queries;
using InvenireServer.Application.Core.Employees.Queries.GetByJwt;
using InvenireServer.Application.Core.Organizations.Invitations.Queries.IndexByEmployee;

namespace InvenireServer.Presentation.Controllers.Queries;

/// <summary>
/// Controller for employee queries.
/// </summary>
[ApiController]
public class EmployeeQueryController : ControllerBase
{
    private readonly IMediator _mediator;

    public EmployeeQueryController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Handles the request to get the current employee profile.
    /// </summary>
    /// <returns>Awaitable task returning the response.</returns>
    [Authorize(Roles = Jwt.Roles.EMPLOYEE)]
    [HttpGet("/api/employees/profile")]
    public async Task<IActionResult> GetByJwt()
    {
        var command = await _mediator.Send(new GetByJwtEmployeeQuery
        {
            Jwt = JwtBuilder.Parse(HttpContext.Request.ParseJwtToken()),
        });

        return Ok(command);
    }

    /// <summary>
    /// Handles the request to list invitations for the current employee.
    /// </summary>
    /// <param name="limit">Pagination limit.</param>
    /// <param name="offset">Pagination offset.</param>
    /// <returns>Awaitable task returning the response.</returns>
    [Authorize(Roles = Jwt.Roles.EMPLOYEE)]
    [HttpGet("/api/employees/invitations")]
    public async Task<IActionResult> GetInvitationsByEmployee([FromQuery] int? limit, [FromQuery] int? offset)
    {
        return Ok(await _mediator.Send(new IndexByEmployeeOrganizationInvitationQuery
        {
            Jwt = JwtBuilder.Parse(HttpContext.Request.ParseJwtToken()),
            Pagination = new QueryPaginationOptions(limit, offset)
        }));
    }
}
