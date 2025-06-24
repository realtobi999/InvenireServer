using InvenireServer.Application.Core.Employees.Commands.Login;
using InvenireServer.Application.Core.Employees.Commands.Register;
using InvenireServer.Application.Core.Employees.Commands.Verification.Confirm;
using InvenireServer.Application.Core.Employees.Commands.Verification.Send;
using InvenireServer.Domain.Entities.Common;
using InvenireServer.Infrastructure.Authentication;
using InvenireServer.Presentation.Extensions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace InvenireServer.Presentation.Controllers;

[ApiController]
public class EmployeeController : ControllerBase
{
    private readonly IConfiguration _configuration;
    private readonly IMediator _mediator;

    public EmployeeController(IMediator mediator, IConfiguration configuration)
    {
        _mediator = mediator;
        _configuration = configuration;
    }

    [HttpPost("/api/auth/employee/register")]
    public async Task<IActionResult> Register([FromBody] RegisterEmployeeCommand command)
    {
        var result = await _mediator.Send(command);

        return Created($"/api/employee/{result.Employee.Id}", result.Token);
    }

    [EnableRateLimiting("SendVerificationPolicy")]
    [Authorize(Policy = Jwt.Policies.UNVERIFIED_EMPLOYEE)]
    [HttpPost("/api/auth/employee/email-verification/send")]
    public async Task<IActionResult> SendVerification()
    {
        var command = new SendVerificationEmployeeCommand
        {
            Jwt = JwtBuilder.Parse(HttpContext.Request.Headers.ParseBearerToken()),
            FrontendBaseUrl = _configuration.GetSection("Frontend:BaseUrl").Value ?? throw new NullReferenceException()
        };
        await _mediator.Send(command);

        return NoContent();
    }

    [Authorize(Policy = Jwt.Policies.UNVERIFIED_EMPLOYEE)]
    [HttpPost("/api/auth/employee/email-verification/confirm")]
    public async Task<IActionResult> ConfirmVerification()
    {
        var command = new ConfirmVerificationEmployeeCommand
        {
            Jwt = JwtBuilder.Parse(HttpContext.Request.Headers.ParseBearerToken())
        };
        await _mediator.Send(command);

        return NoContent();
    }

    [EnableRateLimiting("LoginPolicy")]
    [HttpPost("/api/auth/employee/login")]
    public async Task<IActionResult> Login([FromBody] LoginEmployeeCommand command)
    {
        var result = await _mediator.Send(command);

        return Ok(result.Token);
    }
}