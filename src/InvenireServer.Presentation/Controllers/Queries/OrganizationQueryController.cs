using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using InvenireServer.Domain.Entities.Common;
using InvenireServer.Presentation.Extensions;
using InvenireServer.Infrastructure.Authentication;
using InvenireServer.Application.Core.Employees.Queries.GetById;
using InvenireServer.Application.Core.Organizations.Queries.GetById;
using InvenireServer.Application.Core.Organizations.Queries.GetByAdmin;
using InvenireServer.Application.Core.Employees.Queries.GetByEmailAddress;
using InvenireServer.Application.Core.Organizations.Invitations.Queries.GetById;
using InvenireServer.Application.Core.Organizations.Queries.GetByEmployee;
using InvenireServer.Domain.Exceptions.Http;

namespace InvenireServer.Presentation.Controllers.Queries;

[ApiController]
public class OrganizationQueryController : ControllerBase
{
    private readonly IMediator _mediator;

    public OrganizationQueryController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [Authorize()]
    [HttpGet("/api/organizations")]
    public async Task<IActionResult> GetByJwt()
    {
        var jwt = JwtBuilder.Parse(HttpContext.Request.ParseJwtToken());

        switch (jwt.GetRole())
        {
            case Jwt.Roles.ADMIN:
                return Ok(await _mediator.Send(new GetByAdminOrganizationQuery
                {
                    Jwt = jwt,
                }));
            case Jwt.Roles.EMPLOYEE:
                return Ok(await _mediator.Send(new GetByEmployeeOrganizationQuery
                {
                    Jwt = jwt,
                }));
            default:
                throw new Unauthorized401Exception();
        }
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

    [Authorize(Policy = Jwt.Policies.ADMIN)]
    [HttpGet("/api/organizations/employees/{employeeAddress}")]
    public async Task<IActionResult> GetEmployeeByEmailAddress(string employeeAddress)
    {
        return Ok(await _mediator.Send(new GetByEmailAddressEmployeeQuery
        {
            Jwt = JwtBuilder.Parse(HttpContext.Request.ParseJwtToken()),
            EmployeeEmailAddress = employeeAddress,
        }));
    }
}
