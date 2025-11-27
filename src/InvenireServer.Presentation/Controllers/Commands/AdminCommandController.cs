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