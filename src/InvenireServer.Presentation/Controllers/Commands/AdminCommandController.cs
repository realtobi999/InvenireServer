using MediatR;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;
using InvenireServer.Domain.Constants;
using Microsoft.AspNetCore.Authorization;
using InvenireServer.Domain.Entities.Common;
using InvenireServer.Presentation.Extensions;
using InvenireServer.Infrastructure.Authentication;
using InvenireServer.Application.Core.Admins.Commands.Login;
using InvenireServer.Application.Core.Admins.Commands.Update;
using InvenireServer.Application.Core.Admins.Commands.Delete;
using InvenireServer.Application.Core.Admins.Commands.Register;
using InvenireServer.Application.Core.Admins.Commands.Verification.Send;
using InvenireServer.Application.Core.Admins.Commands.Verification.Confirm;
using InvenireServer.Application.Core.Admins.Commands.Recover.Recover;
using InvenireServer.Application.Core.Admins.Commands.Recover.Send;
using System.Runtime.InteropServices;

namespace InvenireServer.Presentation.Controllers.Commands;

/// <summary>
/// Controller for admin commands.
/// </summary>
[ApiController]
public class AdminCommandController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly IConfiguration _configuration;

    public AdminCommandController(IMediator mediator, IConfiguration configuration)
    {
        _mediator = mediator;
        _configuration = configuration;
    }

    /// <summary>
    /// Handles the request to register an admin.
    /// </summary>
    /// <param name="command">Request to handle.</param>
    /// <returns>Awaitable task returning the response.</returns>
    [HttpPost("/api/admins/register")]
    public async Task<IActionResult> Register([FromBody] RegisterAdminCommand command)
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

        return Created($"/api/admin/{result.Admin.Id}", null);
    }

    // [EnableRateLimiting("SendVerificationPolicy")]
    /// <summary>
    /// Handles the request to send an admin verification email.
    /// </summary>
    /// <returns>Awaitable task returning the response.</returns>
    [Authorize(Policy = Jwt.Policies.UNVERIFIED_ADMIN)]
    [HttpPost("/api/admins/email-verification/send")]
    public async Task<IActionResult> SendVerification()
    {
        await _mediator.Send(new SendVerificationAdminCommand
        {
            Jwt = JwtBuilder.Parse(HttpContext.Request.ParseJwtToken()),
            FrontendBaseAddress = _configuration.GetSection("Frontend:BaseAddress").Value ?? throw new NullReferenceException()
        });

        return NoContent();
    }

    /// <summary>
    /// Handles the request to confirm admin email verification.
    /// </summary>
    /// <returns>Awaitable task returning the response.</returns>
    [Authorize(Policy = Jwt.Policies.UNVERIFIED_ADMIN)]
    [HttpPost("/api/admins/email-verification/confirm")]
    public async Task<IActionResult> ConfirmVerification()
    {
        await _mediator.Send(new ConfirmVerificationAdminCommand
        {
            Jwt = JwtBuilder.Parse(HttpContext.Request.ParseJwtToken())
        });

        return NoContent();
    }

    //[EnableRateLimiting("LoginPolicy")]
    /// <summary>
    /// Handles the request to log in an admin.
    /// </summary>
    /// <param name="command">Request to handle.</param>
    /// <returns>Awaitable task returning the response.</returns>
    [HttpPost("/api/admins/login")]
    public async Task<IActionResult> Login([FromBody] LoginAdminCommand command)
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
    /// Handles the request to send an admin password recovery email.
    /// </summary>
    /// <param name="command">Request to handle.</param>
    /// <returns>Awaitable task returning the response.</returns>
    [HttpPost("/api/admins/password-recovery/send")]
    public async Task<IActionResult> SendPasswordRecovery([FromBody] SendPasswordRecoveryAdminCommand command)
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
    /// Handles the request to recover an admin password.
    /// </summary>
    /// <param name="command">Request to handle.</param>
    /// <returns>Awaitable task returning the response.</returns>
    [Authorize(Roles = Jwt.Roles.ADMIN)]
    [HttpPost("/api/admins/password-recovery/recover")]
    public async Task<IActionResult> RecoverPassword([FromBody] RecoverPasswordAdminCommand command)
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
    /// Handles the request to update the current admin.
    /// </summary>
    /// <param name="command">Request to handle.</param>
    /// <returns>Awaitable task returning the response.</returns>
    [Authorize(Policy = Jwt.Policies.ADMIN)]
    [HttpPut("/api/admins")]
    public async Task<IActionResult> Update([FromBody] UpdateAdminCommand command)
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
    /// Handles the request to delete the current admin.
    /// </summary>
    /// <returns>Awaitable task returning the response.</returns>
    [Authorize(Policy = Jwt.Policies.ADMIN)]
    [HttpDelete("/api/admins")]
    public async Task<IActionResult> Delete()
    {
        await _mediator.Send(new DeleteAdminCommand
        {
            Jwt = JwtBuilder.Parse(HttpContext.Request.ParseJwtToken())
        });

        return NoContent();
    }
}
