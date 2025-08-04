using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using InvenireServer.Domain.Entities.Common;
using InvenireServer.Presentation.Extensions;
using InvenireServer.Infrastructure.Authentication;
using InvenireServer.Application.Core.Properties.Queries.GetByAdmin;
using InvenireServer.Application.Core.Properties.Items.Queries;

namespace InvenireServer.Presentation.Controllers.Queries;

[ApiController]
public class PropertyQueryController : ControllerBase
{
    private readonly IMediator _mediator;

    public PropertyQueryController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [Authorize(Policy = Jwt.Policies.ADMIN)]
    [HttpGet("/api/properties")]
    public async Task<IActionResult> GetByAdmin()
    {
        var propertyDto = await _mediator.Send(new GetByAdminPropertyQuery
        {
            Jwt = JwtBuilder.Parse(HttpContext.Request.Headers.ParseBearerToken()),
        });

        return Ok(propertyDto);
    }

    [Authorize(Policy = Jwt.Policies.ADMIN)]
    [HttpGet("/api/properties/items")]
    public async Task<IActionResult> IndexItemsForAdmin([FromQuery] int limit, [FromQuery] int offset)
    {
        var itemDtos = await _mediator.Send(new IndexForAdminPropertyItemQuery
        {
            Jwt = JwtBuilder.Parse(HttpContext.Request.Headers.ParseBearerToken()),
            Pagination = new PaginationParameters(limit, offset),
        });

        return Ok(itemDtos.ToList());
    }
}
