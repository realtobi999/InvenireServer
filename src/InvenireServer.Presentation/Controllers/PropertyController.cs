using InvenireServer.Application.Core.Properties.Command.Create;
using InvenireServer.Application.Core.Properties.Items.Commands.Create;
using InvenireServer.Domain.Entities.Common;
using InvenireServer.Infrastructure.Authentication;
using InvenireServer.Presentation.Extensions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InvenireServer.Presentation.Controllers;

[ApiController]
public class PropertyController : ControllerBase
{
    private readonly IMediator _mediator;

    public PropertyController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [Authorize(Policy = Jwt.Policies.ADMIN)]
    [HttpPost("/api/organizations/{organizationId:guid}/properties")]
    public async Task<IActionResult> Create([FromBody] CreatePropertyCommand command, Guid organizationId)
    {
        command = command with
        {
            Jwt = JwtBuilder.Parse(HttpContext.Request.Headers.ParseBearerToken()),
            OrganizationId = organizationId
        };
        var result = await _mediator.Send(command);

        return Created($"/api/organizations/{organizationId}/properties/{result.Property.Id}", null);
    }

    [Authorize(Policy = Jwt.Policies.ADMIN)]
    [HttpPost("/api/organizations/{organizationId:guid}/properties/{propertyId:guid}/items")]
    public async Task<IActionResult> CreateItems([FromBody] CreatePropertyItemsCommand command, Guid organizationId, Guid propertyId)
    {
        command = command with
        {
            Jwt = JwtBuilder.Parse(HttpContext.Request.Headers.ParseBearerToken()),
            PropertyId = propertyId,
            OrganizationId = organizationId,
        };
        await _mediator.Send(command);

        return Created();
    }
}
