using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using InvenireServer.Domain.Entities.Common;
using InvenireServer.Presentation.Extensions;
using InvenireServer.Infrastructure.Authentication;
using InvenireServer.Application.Core.Properties.Queries.GetByAdmin;

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
}
