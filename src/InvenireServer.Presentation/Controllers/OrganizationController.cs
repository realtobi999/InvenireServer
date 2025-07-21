using MediatR;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using InvenireServer.Domain.Entities.Common;
using InvenireServer.Presentation.Extensions;
using InvenireServer.Infrastructure.Authentication;
using InvenireServer.Application.Core.Organizations.Commands.Create;
using InvenireServer.Application.Core.Organizations.Invitations.Commands.Accept;
using InvenireServer.Application.Core.Organizations.Invitations.Commands.Create;
using InvenireServer.Application.Core.Organizations.Invitations.Commands.Delete;
using InvenireServer.Application.Core.Organizations.Commands.Remove;
using InvenireServer.Application.Core.Organizations.Commands.Update;
using InvenireServer.Application.Core.Organizations.Commands.Delete;
using InvenireServer.Application.Core.Organizations.Invitations.Commands.Update;

namespace InvenireServer.Presentation.Controllers;

[ApiController]
public class OrganizationController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly IConfiguration _configuration;

    public OrganizationController(IMediator mediator, IConfiguration configuration)
    {
        _mediator = mediator;
        _configuration = configuration;
    }

    [Authorize(Policy = Jwt.Policies.ADMIN)]
    [HttpPost("/api/organizations")]
    public async Task<IActionResult> Create([FromBody] CreateOrganizationCommand command)
    {
        if (command is null)
            throw new ValidationException([new ValidationFailure("", "Request body is missing or invalid.")]);

        command = command with
        {
            Jwt = JwtBuilder.Parse(HttpContext.Request.Headers.ParseBearerToken()),
            FrontendBaseUrl = _configuration.GetSection("Frontend:BaseUrl").Value ?? throw new NullReferenceException()
        };
        var result = await _mediator.Send(command);

        return Created($"/api/organizations/{result.Organization.Id}", null);
    }

    [Authorize(Policy = Jwt.Policies.ADMIN)]
    [HttpPut("/api/organizations")]
    public async Task<IActionResult> Update([FromBody] UpdateOrganizationCommand command)
    {
        if (command is null)
            throw new ValidationException([new ValidationFailure("", "Request body is missing or invalid.")]);

        command = command with
        {
            Jwt = JwtBuilder.Parse(HttpContext.Request.Headers.ParseBearerToken()),
        };
        await _mediator.Send(command);

        return NoContent();
    }

    [Authorize(Policy = Jwt.Policies.ADMIN)]
    [HttpDelete("/api/organizations")]
    public async Task<IActionResult> Delete()
    {
        await _mediator.Send(new DeleteOrganizationCommand
        {
            Jwt = JwtBuilder.Parse(HttpContext.Request.Headers.ParseBearerToken()),
        });

        return NoContent();
    }

    [Authorize(Policy = Jwt.Policies.ADMIN)]
    [HttpPost("/api/organizations/invitations")]
    public async Task<IActionResult> CreateInvitation([FromBody] CreateOrganizationInvitationCommand command)
    {
        if (command is null)
            throw new ValidationException([new ValidationFailure("", "Request body is missing or invalid.")]);

        command = command with
        {
            Jwt = JwtBuilder.Parse(HttpContext.Request.Headers.ParseBearerToken())
        };
        var result = await _mediator.Send(command);

        return Created($"/api/organizations/{result.Organization.Id}/invitations/{result.Invitation.Id}", null);
    }

    [Authorize(Policy = Jwt.Policies.ADMIN)]
    [HttpPut("/api/organizations/invitations/{invitationId:guid}")]
    public async Task<IActionResult> UpdateInvitation([FromBody] UpdateOrganizationInvitationCommand command, Guid invitationId)
    {
        if (command is null)
            throw new ValidationException([new ValidationFailure("", "Request body is missing or invalid.")]);

        command = command with
        {
            Jwt = JwtBuilder.Parse(HttpContext.Request.Headers.ParseBearerToken()),
            InvitationId = invitationId
        };
        await _mediator.Send(command);

        return NoContent();
    }

    [Authorize(Policy = Jwt.Policies.ADMIN)]
    [HttpDelete("/api/organizations/invitations/{invitationId:guid}")]
    public async Task<IActionResult> DeleteInvitation(Guid invitationId)
    {
        await _mediator.Send(new DeleteOrganizationInvitationCommand
        {
            Id = invitationId,
            Jwt = JwtBuilder.Parse(HttpContext.Request.Headers.ParseBearerToken())
        });

        return NoContent();
    }

    [Authorize(Policy = Jwt.Policies.EMPLOYEE)]
    [HttpPut("/api/organizations/invitations/{invitationId:guid}/accept")]
    public async Task<IActionResult> AcceptInvitation(Guid invitationId)
    {
        await _mediator.Send(new AcceptOrganizationInvitationCommand
        {
            Jwt = JwtBuilder.Parse(HttpContext.Request.Headers.ParseBearerToken()),
            InvitationId = invitationId
        });

        return NoContent();
    }

    [Authorize(Policy = Jwt.Policies.ADMIN)]
    [HttpDelete("/api/organizations/employees/{employeeId:guid}")]
    public async Task<IActionResult> RemoveEmployee(Guid employeeId)
    {
        await _mediator.Send(new RemoveOrganizationEmployeeCommand
        {
            Jwt = JwtBuilder.Parse(HttpContext.Request.Headers.ParseBearerToken()),
            EmployeeId = employeeId
        });

        return NoContent();
    }
}