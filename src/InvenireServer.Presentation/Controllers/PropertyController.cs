using InvenireServer.Application.Core.Properties.Commands.Create;
using InvenireServer.Application.Core.Properties.Items.Commands.Create;
using InvenireServer.Application.Core.Properties.Items.Commands.Delete;
using InvenireServer.Application.Core.Properties.Items.Commands.Update;
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
    [HttpPost("/api/properties")]
    public async Task<IActionResult> Create([FromBody] CreatePropertyCommand command)
    {
        command = command with
        {
            Jwt = JwtBuilder.Parse(HttpContext.Request.Headers.ParseBearerToken()),
        };
        var result = await _mediator.Send(command);

        return Created($"/api/properties/{result.Property.Id}", null);
    }

    [Authorize(Policy = Jwt.Policies.ADMIN)]
    [HttpPost("/api/properties/items")]
    public async Task<IActionResult> CreateItems([FromBody] CreatePropertyItemsCommand command)
    {
        command = command with
        {
            Jwt = JwtBuilder.Parse(HttpContext.Request.Headers.ParseBearerToken()),
        };
        await _mediator.Send(command);

        return Created();
    }

    [Authorize(Policy = Jwt.Policies.ADMIN)]
    [HttpPut("/api/properties/items")]
    public async Task<IActionResult> UpdateItems([FromBody] UpdatePropertyItemsCommand command)
    {
        command = command with
        {
            Jwt = JwtBuilder.Parse(HttpContext.Request.Headers.ParseBearerToken()),
        };
        await _mediator.Send(command);

        return NoContent();
    }

    [Authorize(Policy = Jwt.Policies.ADMIN)]
    [HttpDelete("/api/properties/items")]
    public async Task<IActionResult> DeleteItems([FromQuery] List<Guid> ids)
    {
        await _mediator.Send(new DeletePropertyItemsCommand
        {
            Jwt = JwtBuilder.Parse(HttpContext.Request.Headers.ParseBearerToken()),
            Ids = ids,
        });

        return NoContent();
    }
}