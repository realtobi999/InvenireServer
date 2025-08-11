using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using InvenireServer.Domain.Entities.Common;
using InvenireServer.Presentation.Extensions;
using InvenireServer.Infrastructure.Authentication;
using InvenireServer.Application.Core.Properties.Queries.GetByAdmin;
using InvenireServer.Application.Core.Properties.Scans.Queries.IndexByAdmin;
using InvenireServer.Application.Core.Properties.Items.Queries.IndexByAdmin;

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
            Jwt = JwtBuilder.Parse(HttpContext.Request.Headers.ParseBearerToken()),
        }));
    }

    [Authorize(Policy = Jwt.Policies.ADMIN)]
    [HttpGet("/api/properties/items")]
    public async Task<IActionResult> IndexItemsForAdmin([FromQuery] int? limit, [FromQuery] int? offset)
    {

        return Ok(await _mediator.Send(new IndexByAdminPropertyItemQuery
        {
            Jwt = JwtBuilder.Parse(HttpContext.Request.Headers.ParseBearerToken()),
            Pagination = new PaginationParameters(limit, offset),
        }));
    }

    [Authorize(Policy = Jwt.Policies.ADMIN)]
    [HttpGet("/api/properties/scans")]
    public async Task<IActionResult> IndexScansForAdmin([FromQuery] int? limit, [FromQuery] int? offset)
    {
        return Ok(await _mediator.Send(new IndexByAdminPropertyScanQuery
        {
            Jwt = JwtBuilder.Parse(HttpContext.Request.Headers.ParseBearerToken()),
            Pagination = new PaginationParameters(limit, offset),
        }));
    }
}
