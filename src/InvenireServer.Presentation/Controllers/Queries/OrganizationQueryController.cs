using InvenireServer.Application.Core.Employees.Queries.GetById;
using InvenireServer.Application.Core.Organizations.Invitations.Queries.GetByEmployee;
using InvenireServer.Application.Core.Organizations.Invitations.Queries.GetById;
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

    [Authorize(Policy = Jwt.Policies.ADMIN)]
    [HttpGet("/api/organizations/invitations/{invitationId:guid}")]
    public async Task<IActionResult> GetInvitationById(Guid invitationId)
    {
        var invitationDto = await _mediator.Send(new GetByIdOrganizationInvitationQuery
        {
            Jwt = JwtBuilder.Parse(HttpContext.Request.Headers.ParseBearerToken()),
            InvitationId = invitationId
        });

        return Ok(invitationDto);
    }
}
