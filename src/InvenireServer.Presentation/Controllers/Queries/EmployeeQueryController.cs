using InvenireServer.Application.Core.Employees.Queries.GetById;
using InvenireServer.Application.Core.Employees.Queries.GetByJwt;
using InvenireServer.Domain.Entities.Common;
using InvenireServer.Infrastructure.Authentication;
using InvenireServer.Presentation.Extensions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InvenireServer.Presentation.Controllers.Queries;

[ApiController]
public class EmployeeQueryController : ControllerBase
{
    private readonly IMediator _mediator;

    public EmployeeQueryController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [Authorize(Roles = Jwt.Roles.EMPLOYEE)]
    [HttpGet("/api/employees/profile")]
    public async Task<IActionResult> GetByJwt()
    {
        var employeeDto = await _mediator.Send(new GetByJwtEmployeeQuery
        {
            Jwt = JwtBuilder.Parse(HttpContext.Request.Headers.ParseBearerToken()),
        });

        return Ok(employeeDto);
    }

    [Authorize(Policy = Jwt.Policies.ADMIN)]
    [HttpGet("/api/employees/{employeeId:guid}")]
    public async Task<IActionResult> GetById(Guid employeeId)
    {
        var employeeDto = await _mediator.Send(new GetByIdEmployeeQuery
        {
            Jwt = JwtBuilder.Parse(HttpContext.Request.Headers.ParseBearerToken()),
            EmployeeId = employeeId,
        });

        return Ok(employeeDto);
    }
}
