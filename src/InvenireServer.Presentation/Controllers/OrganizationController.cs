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
        command = command with
        {
            Jwt = JwtBuilder.Parse(HttpContext.Request.Headers.ParseBearerToken()),
            FrontendBaseUrl = _configuration.GetSection("Frontend:BaseUrl").Value ?? throw new NullReferenceException()
        };
        var result = await _mediator.Send(command);

        return Created($"/api/organizations/{result.Organization.Id}", null);
    }

    [Authorize(Policy = Jwt.Policies.ADMIN)]
    [HttpPost("/api/organizations/{organizationId:guid}/invitations")]
    public async Task<IActionResult> CreateInvitation(Guid organizationId, [FromBody] CreateOrganizationInvitationCommand command)
    {
        command = command with
        {
            Jwt = JwtBuilder.Parse(HttpContext.Request.Headers.ParseBearerToken()),
            OrganizationId = organizationId
        };
        var result = await _mediator.Send(command);

        return Created($"/api/organizations/{result.Organization.Id}/invitations/{result.Invitation.Id}", null);
    }

    [Authorize(Policy = Jwt.Policies.EMPLOYEE)]
    [HttpPost("/api/organizations/{organizationId:guid}/invitations/{invitationId:guid}/accept")]
    public async Task<IActionResult> AcceptInvitation(Guid organizationId, Guid invitationId)
    {
        var command = new AcceptOrganizationInvitationCommand
        {
            Jwt = JwtBuilder.Parse(HttpContext.Request.Headers.ParseBearerToken()),
            InvitationId = invitationId,
            OrganizationId = organizationId
        };
        await _mediator.Send(command);

        return NoContent();
    }
}