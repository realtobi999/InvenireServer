using InvenireServer.Application.Core.Admins.Queries.GetByJwt;
using InvenireServer.Domain.Entities.Common;
using InvenireServer.Infrastructure.Authentication;
using InvenireServer.Presentation.Extensions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InvenireServer.Presentation.Controllers.Queries;

[ApiController]
public class AdminQueryController : ControllerBase
{
    private readonly IMediator _mediator;

    public AdminQueryController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [Authorize(Roles = Jwt.Roles.ADMIN)]
    [HttpGet("/api/admins/profile")]
    public async Task<IActionResult> GetByJwt()
    {
        var adminDto = await _mediator.Send(new GetByJwtAdminQuery
        {
            Jwt = JwtBuilder.Parse(HttpContext.Request.Headers.ParseBearerToken()),
        });

        return Ok(adminDto);
    }
}
