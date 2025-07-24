using InvenireServer.Application.Core.Organizations.Queries.GetByAdmin;
using InvenireServer.Domain.Entities.Common;
using InvenireServer.Infrastructure.Authentication;
using InvenireServer.Presentation.Extensions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InvenireServer.Presentation.Controllers.Queries;

[ApiController]
public class OrganizationQueryController : ControllerBase
{
    private readonly IMediator _mediator;

    public OrganizationQueryController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [Authorize(Policy = Jwt.Policies.ADMIN)]
    [HttpGet("/api/organizations")]
    public async Task<IActionResult> GetByAdmin()
    {
        var organizationDto = await _mediator.Send(new GetByAdminOrganizationQuery
        {
            Jwt = JwtBuilder.Parse(HttpContext.Request.Headers.ParseBearerToken()),
        });

        return Ok(organizationDto);
    }
}
