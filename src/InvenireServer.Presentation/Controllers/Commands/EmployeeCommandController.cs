using FluentValidation;
using FluentValidation.Results;
using InvenireServer.Application.Core.Employees.Commands.Delete;
using InvenireServer.Application.Core.Employees.Commands.Login;
using InvenireServer.Application.Core.Employees.Commands.Recover.Recover;
using InvenireServer.Application.Core.Employees.Commands.Recover.Send;
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

/// <summary>
/// Controller for employee commands.
/// </summary>
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

    /// <summary>
    /// Handles the request to register an employee.
    /// </summary>
    /// <param name="command">Request to handle.</param>
    /// <returns>Awaitable task returning the response.</returns>
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
    /// <summary>
    /// Handles the request to send an employee verification email.
    /// </summary>
    /// <returns>Awaitable task returning the response.</returns>
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

    /// <summary>
    /// Handles the request to confirm employee email verification.
    /// </summary>
    /// <returns>Awaitable task returning the response.</returns>
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

    /// <summary>
    /// Handles the request to log in an employee.
    /// </summary>
    /// <param name="command">Request to handle.</param>
    /// <returns>Awaitable task returning the response.</returns>
    [EnableRateLimiting("LoginPolicy")]
    [HttpPost("/api/employees/login")]
    public async Task<IActionResult> Login([FromBody] LoginEmployeeCommand command)
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

        return NoContent();
    }

    /// <summary>
    /// Handles the request to send an employee password recovery email.
    /// </summary>
    /// <param name="command">Request to handle.</param>
    /// <returns>Awaitable task returning the response.</returns>
    [HttpPost("/api/employees/password-recovery/send")]
    public async Task<IActionResult> SendPasswordRecovery([FromBody] SendPasswordRecoveryEmployeeCommand command)
    {
        if (command is null)
            throw new ValidationException([new ValidationFailure("", "Request body is missing or invalid.")]);

        command = command with
        {
            FrontendBaseAddress = _configuration.GetSection("Frontend:BaseAddress").Value ?? throw new NullReferenceException()
        };
        await _mediator.Send(command);

        return NoContent();
    }

    /// <summary>
    /// Handles the request to recover an employee password.
    /// </summary>
    /// <param name="command">Request to handle.</param>
    /// <returns>Awaitable task returning the response.</returns>
    [Authorize(Roles = Jwt.Roles.EMPLOYEE)]
    [HttpPost("/api/employees/password-recovery/recover")]
    public async Task<IActionResult> RecoverPassword([FromBody] RecoverPasswordEmployeeCommand command)
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

    /// <summary>
    /// Handles the request to update the current employee.
    /// </summary>
    /// <param name="command">Request to handle.</param>
    /// <returns>Awaitable task returning the response.</returns>
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
    /// <summary>
    /// Handles the request to delete the current employee.
    /// </summary>
    /// <returns>Awaitable task returning the response.</returns>
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
