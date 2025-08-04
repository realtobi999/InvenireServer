using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using InvenireServer.Domain.Entities.Common;
using InvenireServer.Presentation.Extensions;
using InvenireServer.Infrastructure.Authentication;
using InvenireServer.Application.Core.Employees.Queries.GetByJwt;
using InvenireServer.Application.Core.Organizations.Invitations.Queries.GetByEmployee;

namespace InvenireServer.Presentation.Controllers.Queries;

[ApiController]
public class EmployeeQueryController : ControllerBase
{
    private readonly IMediator _mediator;

    public EmployeeQueryController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [Authorize(Roles = Jwt.Roles.EMPLOYEE)]
    [HttpGet("/api/employees/profile")]
    public async Task<IActionResult> GetByJwt()
    {
        var employeeDto = await _mediator.Send(new GetByJwtEmployeeQuery
        {
            Jwt = JwtBuilder.Parse(HttpContext.Request.Headers.ParseBearerToken()),
        });

        return Ok(employeeDto);
    }

    [Authorize(Roles = Jwt.Roles.EMPLOYEE)]
    [HttpGet("/api/employees/invitations")]
    public async Task<IActionResult> GetInvitationsByEmployee()
    {
        var invitationDtos = await _mediator.Send(new GetByEmployeeOrganizationInvitationQuery
        {
            Jwt = JwtBuilder.Parse(HttpContext.Request.Headers.ParseBearerToken()),
        });

        return Ok(invitationDtos.ToList());
    }
}
