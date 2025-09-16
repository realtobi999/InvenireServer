using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using InvenireServer.Domain.Entities.Common;
using InvenireServer.Presentation.Extensions;
using InvenireServer.Infrastructure.Authentication;
using InvenireServer.Application.Core.Properties.Queries.GetByAdmin;
using InvenireServer.Application.Core.Properties.Scans.Queries.IndexByAdmin;
using InvenireServer.Application.Core.Properties.Items.Queries.IndexByAdmin;
using InvenireServer.Application.Core.Properties.Suggestions.Queries.IndexByAdmin;
using InvenireServer.Domain.Entities.Common.Queries;
using InvenireServer.Domain.Entities.Properties;
using InvenireServer.Application.Core.Properties.Items.Queries.GetById;

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
        return Ok(await _mediator.Send(new GetByAdminPropertyQuery
        {
            Jwt = JwtBuilder.Parse(HttpContext.Request.ParseJwtToken()),
        }));
    }

    [Authorize(Policy = Jwt.Policies.ADMIN)]
    [HttpGet("/api/properties/items")]
    public async Task<IActionResult> IndexItemsByAdmin([FromQuery] IndexByAdminPropertyItemQueryParameters parameters)
    {
        return Ok(await _mediator.Send(new IndexByAdminPropertyItemQuery
        {
            Jwt = JwtBuilder.Parse(HttpContext.Request.ParseJwtToken()),
            Parameters = parameters
        }));
    }

    [Authorize(Policy = Jwt.Policies.ADMIN)]
    [HttpGet("/api/properties/items/{itemId:guid}")]
    public async Task<IActionResult> GetItemById(Guid itemId)
    {
        return Ok(await _mediator.Send(new GetByIdPropertyItemQuery
        {
            Jwt = JwtBuilder.Parse(HttpContext.Request.ParseJwtToken()),
            ItemId = itemId,
        }));
    }

    [Authorize(Policy = Jwt.Policies.ADMIN)]
    [HttpGet("/api/properties/scans")]
    public async Task<IActionResult> IndexScansByAdmin([FromQuery] int? limit, [FromQuery] int? offset)
    {
        return Ok(await _mediator.Send(new IndexByAdminPropertyScanQuery
        {
            Jwt = JwtBuilder.Parse(HttpContext.Request.ParseJwtToken()),
            Pagination = new QueryPaginationOptions(limit, offset),
        }));
    }

    [Authorize(Policy = Jwt.Policies.ADMIN)]
    [HttpGet("/api/properties/suggestions")]
    public async Task<IActionResult> IndexSuggestionsByAdmin([FromQuery] IndexByAdminPropertySuggestionQueryParameters parameters)
    {
        Console.WriteLine("Status: " + parameters.Status.ToString());
        return Ok(await _mediator.Send(new IndexByAdminPropertySuggestionQuery
        {
            Jwt = JwtBuilder.Parse(HttpContext.Request.ParseJwtToken()),
            Parameters = parameters
        }));
    }
}
