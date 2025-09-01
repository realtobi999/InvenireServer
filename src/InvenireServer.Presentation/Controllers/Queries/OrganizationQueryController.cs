using InvenireServer.Application.Core.Employees.Queries.GetById;
using InvenireServer.Application.Core.Organizations.Invitations.Queries.GetById;
using InvenireServer.Application.Core.Organizations.Queries.GetByAdmin;
using InvenireServer.Application.Core.Organizations.Queries.GetById;
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
        return Ok(await _mediator.Send(new GetByAdminOrganizationQuery
        {
            Jwt = JwtBuilder.Parse(HttpContext.Request.ParseJwtToken()),
        }));
    }

    [HttpGet("/api/organizations/{organizationId:guid}")]
    public async Task<IActionResult> GetById(Guid organizationId)
    {
        return Ok(await _mediator.Send(new GetByIdOrganizationQuery
        {
            OrganizationId = organizationId
        }));
    }

    [Authorize(Policy = Jwt.Policies.ADMIN)]
    [HttpGet("/api/organizations/invitations/{invitationId:guid}")]
    public async Task<IActionResult> GetInvitationById(Guid invitationId)
    {
        return Ok(await _mediator.Send(new GetByIdOrganizationInvitationQuery
        {
            Jwt = JwtBuilder.Parse(HttpContext.Request.ParseJwtToken()),
            InvitationId = invitationId
        }));
    }

    [Authorize(Policy = Jwt.Policies.ADMIN)]
    [HttpGet("/api/organizations/employees/{employeeId:guid}")]
    public async Task<IActionResult> GetEmployeeById(Guid employeeId)
    {
        return Ok(await _mediator.Send(new GetByIdEmployeeQuery
        {
            Jwt = JwtBuilder.Parse(HttpContext.Request.ParseJwtToken()),
            EmployeeId = employeeId,
        }));
    }
}
