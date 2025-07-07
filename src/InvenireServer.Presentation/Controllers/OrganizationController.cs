using FluentValidation;
using FluentValidation.Results;
using InvenireServer.Application.Core.Organizations.Commands.Create;
using InvenireServer.Application.Core.Organizations.Invitations.Commands.Accept;
using InvenireServer.Application.Core.Organizations.Invitations.Commands.Create;
using InvenireServer.Domain.Entities.Common;
using InvenireServer.Infrastructure.Authentication;
using InvenireServer.Presentation.Extensions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InvenireServer.Presentation.Controllers;

[ApiController]
public class OrganizationController : ControllerBase
{
    private readonly IConfiguration _configuration;
    private readonly IMediator _mediator;

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

    [Authorize(Policy = Jwt.Policies.EMPLOYEE)]
    [HttpPost("/api/organizations/{organizationId:guid}/invitations/accept")]
    public async Task<IActionResult> AcceptInvitation(Guid organizationId)
    {
        await _mediator.Send(new AcceptOrganizationInvitationCommand
        {
            Jwt = JwtBuilder.Parse(HttpContext.Request.Headers.ParseBearerToken()),
            OrganizationId = organizationId
        });

        return NoContent();
    }
}