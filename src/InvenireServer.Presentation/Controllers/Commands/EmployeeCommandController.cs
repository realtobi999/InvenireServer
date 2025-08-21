using FluentValidation;
using FluentValidation.Results;
using InvenireServer.Application.Core.Employees.Commands.Delete;
using InvenireServer.Application.Core.Employees.Commands.Login;
using InvenireServer.Application.Core.Employees.Commands.Register;
using InvenireServer.Application.Core.Employees.Commands.Update;
using InvenireServer.Application.Core.Employees.Commands.Verification.Confirm;
using InvenireServer.Application.Core.Employees.Commands.Verification.Send;
using InvenireServer.Domain.Constants;
using InvenireServer.Domain.Entities.Common;
using InvenireServer.Infrastructure.Authentication;
using InvenireServer.Presentation.Extensions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace InvenireServer.Presentation.Controllers.Commands;

[ApiController]
public class EmployeeCommandController : ControllerBase
{
    private readonly IConfiguration _configuration;
    private readonly IMediator _mediator;

    public EmployeeCommandController(IMediator mediator, IConfiguration configuration)
    {
        _mediator = mediator;
        _configuration = configuration;
    }

    [HttpPost("/api/employees/register")]
    public async Task<IActionResult> Register([FromBody] RegisterEmployeeCommand command)
    {
        if (command is null)
            throw new ValidationException([new ValidationFailure("", "Request body is missing or invalid.")]);

        var result = await _mediator.Send(command);

        Response.Cookies.Append(CookieConstants.JWT, result.TokenString, new CookieOptions
        {
            Secure = true,
            Expires = result.Token.GetExpirationTime(),
            SameSite = SameSiteMode.None,
            HttpOnly = true,
        });

        return Created($"/api/admin/{result.Employee.Id}", null);
    }

    // [EnableRateLimiting("SendVerificationPolicy")]
    [Authorize(Policy = Jwt.Policies.UNVERIFIED_EMPLOYEE)]
    [HttpPost("/api/employees/email-verification/send")]
    public async Task<IActionResult> SendVerification()
    {
        await _mediator.Send(new SendVerificationEmployeeCommand
        {
            Jwt = JwtBuilder.Parse(HttpContext.Request.ParseJwtToken()),
            FrontendBaseAddress = _configuration.GetSection("Frontend:BaseAddress").Value ?? throw new NullReferenceException()
        });

        return NoContent();
    }

    [Authorize(Policy = Jwt.Policies.UNVERIFIED_EMPLOYEE)]
    [HttpPost("/api/employees/email-verification/confirm")]
    public async Task<IActionResult> ConfirmVerification()
    {
        await _mediator.Send(new ConfirmVerificationEmployeeCommand
        {
            Jwt = JwtBuilder.Parse(HttpContext.Request.ParseJwtToken())
        });

        return NoContent();
    }

    [EnableRateLimiting("LoginPolicy")]
    [HttpPost("/api/employees/login")]
    public async Task<IActionResult> Login([FromBody] LoginEmployeeCommand command)
    {
        if (command is null)
            throw new ValidationException([new ValidationFailure("", "Request body is missing or invalid.")]);

        return Ok(await _mediator.Send(command));
    }

    [Authorize(Policy = Jwt.Policies.EMPLOYEE)]
    [HttpPut("/api/employees")]
    public async Task<IActionResult> Update([FromBody] UpdateEmployeeCommand command)
    {
        if (command is null)
            throw new ValidationException([new ValidationFailure("", "Request body is missing or invalid.")]);

        command = command with
        {
            Jwt = JwtBuilder.Parse(HttpContext.Request.ParseJwtToken())
        };
        await _mediator.Send(command);

        return NoContent();

    }
    [Authorize(Policy = Jwt.Policies.EMPLOYEE)]
    [HttpDelete("/api/employees")]
    public async Task<IActionResult> Delete()
    {
        await _mediator.Send(new DeleteEmployeeCommand
        {
            Jwt = JwtBuilder.Parse(HttpContext.Request.ParseJwtToken())
        });

        return NoContent();
    }
}