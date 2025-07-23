using InvenireServer.Application.Core.Admins.Queries.GetById;
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
    [HttpGet("/api/admins/me")]
    public async Task<IActionResult> GetByJwt()
    {
        var result = await _mediator.Send(new GetByJwtAdminQuery
        {
            Jwt = JwtBuilder.Parse(HttpContext.Request.Headers.ParseBearerToken()),
        });

        return Ok(result.AdminDto);
    }

    [HttpGet("/api/admins/{adminId:guid}")]
    public async Task<IActionResult> GetById(Guid adminId)
    {
        var result = await _mediator.Send(new GetByIdAdminQuery
        {
            AdminId = adminId
        });

        return Ok(result.AdminDto);
    }
}
