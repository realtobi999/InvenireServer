using MediatR;
using System.Globalization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using InvenireServer.Domain.Exceptions.Http;
using InvenireServer.Domain.Entities.Common;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using InvenireServer.Presentation.Extensions;
using InvenireServer.Infrastructure.Authentication;
using InvenireServer.Application.Core.Properties.Queries.GetByAdmin;
using InvenireServer.Application.Core.Properties.Items.Queries.GetById;
using InvenireServer.Application.Core.Properties.Queries.GetByEmployee;
using InvenireServer.Application.Core.Properties.Items.Queries.IndexByScan;
using InvenireServer.Application.Core.Properties.Scans.Queries.IndexByAdmin;
using InvenireServer.Application.Core.Properties.Items.Queries.IndexByAdmin;
using InvenireServer.Application.Core.Properties.Items.Queries.IndexByEmployee;
using InvenireServer.Application.Core.Properties.Suggestions.Queries.IndexByAdmin;
using InvenireServer.Application.Core.Properties.Suggestions.Queries.IndexByEmployee;
using InvenireServer.Application.Core.Properties.Scans.Queries.GetActive;

namespace InvenireServer.Presentation.Controllers.Queries;

[ApiController]
public class PropertyQueryController : ControllerBase
{
    private readonly IMediator _mediator;

    public PropertyQueryController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [Authorize()]
    [HttpGet("/api/properties")]
    public async Task<IActionResult> GetByJwt()
    {
        var jwt = JwtBuilder.Parse(HttpContext.Request.ParseJwtToken());

        switch (jwt.GetRole())
        {
            case Jwt.Roles.ADMIN:
                return Ok(await _mediator.Send(new GetByAdminPropertyQuery
                {
                    Jwt = jwt,
                }));
            case Jwt.Roles.EMPLOYEE:
                return Ok(await _mediator.Send(new GetByEmployeePropertyQuery
                {
                    Jwt = jwt,
                }));
            default:
                throw new Unauthorized401Exception();
        }
    }

    [Authorize()]
    [HttpGet("/api/properties/items")]
    public async Task<IActionResult> IndexItemsByJwt()
    {
        var jwt = JwtBuilder.Parse(HttpContext.Request.ParseJwtToken());

        // Each role requires slightly different parameters, so  we  parse  them
        // manually instead of relying on automatic ASP.NET model binding.
        switch (jwt.GetRole())
        {
            case Jwt.Roles.ADMIN:
                {
                    var parameters = new IndexByAdminPropertyItemQueryParameters();
                    await TryUpdateModelAsync(parameters, prefix: "", new QueryStringValueProvider(BindingSource.Query, HttpContext.Request.Query, CultureInfo.InvariantCulture));

                    return Ok(await _mediator.Send(new IndexByAdminPropertyItemQuery
                    {
                        Jwt = jwt,
                        Parameters = parameters,
                    }));

                }
            case Jwt.Roles.EMPLOYEE:
                {
                    var parameters = new IndexByEmployeePropertyItemQueryParameters();
                    await TryUpdateModelAsync(parameters, prefix: "", new QueryStringValueProvider(BindingSource.Query, HttpContext.Request.Query, CultureInfo.InvariantCulture));

                    return Ok(await _mediator.Send(new IndexByEmployeePropertyItemQuery
                    {
                        Jwt = jwt,
                        Parameters = parameters
                    }));

                }
            default:
                throw new Unauthorized401Exception();
        }
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
    public async Task<IActionResult> IndexScansByAdmin([FromQuery] IndexByAdminPropertyScanQueryParameters parameters)
    {
        return Ok(await _mediator.Send(new IndexByAdminPropertyScanQuery
        {
            Jwt = JwtBuilder.Parse(HttpContext.Request.ParseJwtToken()),
            Parameters = parameters
        }));
    }

    [Authorize()]
    [HttpGet("/api/properties/scans/active")]
    public async Task<IActionResult> GetActiveScan()
    {
        return Ok(await _mediator.Send(new GetActivePropertyScanQuery
        {
            Jwt = JwtBuilder.Parse(HttpContext.Request.ParseJwtToken()),
        }));
    }

    [Authorize()]
    [HttpGet("/api/properties/scans/{scanId:guid}/items")]
    public async Task<IActionResult> IndexItemsByScan(Guid scanId, [FromQuery] IndexByScanPropertyItemQueryParameters parameters)
    {
        return Ok(await _mediator.Send(new IndexByScanPropertyItemQuery
        {
            Jwt = JwtBuilder.Parse(HttpContext.Request.ParseJwtToken()),
            ScanId = scanId,
            Parameters = parameters,
        }));
    }

    [Authorize()]
    [HttpGet("/api/properties/suggestions")]
    public async Task<IActionResult> IndexSuggestionsByJwt()
    {
        var jwt = JwtBuilder.Parse(HttpContext.Request.ParseJwtToken());

        // Each role requires slightly different parameters, so  we  parse  them
        // manually instead of relying on automatic ASP.NET model binding.
        switch (jwt.GetRole())
        {
            case Jwt.Roles.ADMIN:
                {
                    var parameters = new IndexByAdminPropertySuggestionQueryParameters();
                    await TryUpdateModelAsync(parameters, prefix: "", new QueryStringValueProvider(BindingSource.Query, HttpContext.Request.Query, CultureInfo.InvariantCulture));

                    return Ok(await _mediator.Send(new IndexByAdminPropertySuggestionQuery
                    {
                        Jwt = jwt,
                        Parameters = parameters,
                    }));
                }
            case Jwt.Roles.EMPLOYEE:
                {
                    var parameters = new IndexByEmployeePropertySuggestionQueryParameters();
                    await TryUpdateModelAsync(parameters, prefix: "", new QueryStringValueProvider(BindingSource.Query, HttpContext.Request.Query, CultureInfo.InvariantCulture));

                    return Ok(await _mediator.Send(new IndexByEmployeePropertySuggestionQuery
                    {
                        Jwt = jwt,
                        Parameters = parameters
                    }));
                }
            default:
                throw new Unauthorized401Exception();
        }
    }
}
