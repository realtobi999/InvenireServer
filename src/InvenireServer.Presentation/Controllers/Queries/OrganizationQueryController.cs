using InvenireServer.Application.Core.Employees.Queries.GetById;
using InvenireServer.Application.Core.Organizations.Invitations.Queries.GetByEmployee;
using InvenireServer.Application.Core.Organizations.Queries.GetByAdmin;
using InvenireServer.Domain.Entities.Common;
using InvenireServer.Infrastructure.Authentication;
using InvenireServer.Presentation.Extensions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InvenireServer.Presentation.Controllers.Queries;

[ApiController]
public class OrganizationQueryController : ControllerBase
{
    private readonly IMediator _mediator;

    public OrganizationQueryController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [Authorize(Policy = Jwt.Policies.ADMIN)]
    [HttpGet("/api/organizations")]
    public async Task<IActionResult> GetByAdmin()
    {
        var organizationDto = await _mediator.Send(new GetByAdminOrganizationQuery
        {
            Jwt = JwtBuilder.Parse(HttpContext.Request.Headers.ParseBearerToken()),
        });

        return Ok(organizationDto);
    }

    [Authorize(Policy = Jwt.Policies.ADMIN)]
    [HttpGet("/api/organizations/employees/{employeeId:guid}")]
    public async Task<IActionResult> GetEmployeeById(Guid employeeId)
    {
        var employeeDto = await _mediator.Send(new GetByIdEmployeeQuery
        {
            Jwt = JwtBuilder.Parse(HttpContext.Request.Headers.ParseBearerToken()),
            EmployeeId = employeeId,
        });

        return Ok(employeeDto);
    }

    [Authorize(Policy = Jwt.Policies.EMPLOYEE)]
    [HttpGet("/api/organizations/employees/invitations")]
    public async Task<IActionResult> GetInvitationsByEmployee()
    {
        var invitationDtos = await _mediator.Send(new GetByEmployeeOrganizationInvitationQuery
        {
            Jwt = JwtBuilder.Parse(HttpContext.Request.Headers.ParseBearerToken()),
        });

        return Ok(invitationDtos.ToList());
    }
}
