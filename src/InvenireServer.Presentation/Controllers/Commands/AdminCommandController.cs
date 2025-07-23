using FluentValidation;
using FluentValidation.Results;
using InvenireServer.Application.Core.Admins.Commands.Delete;
using InvenireServer.Application.Core.Admins.Commands.Login;
using InvenireServer.Application.Core.Admins.Commands.Register;
using InvenireServer.Application.Core.Admins.Commands.Update;
using InvenireServer.Application.Core.Admins.Commands.Verification.Confirm;
using InvenireServer.Application.Core.Admins.Commands.Verification.Send;
using InvenireServer.Domain.Entities.Common;
using InvenireServer.Infrastructure.Authentication;
using InvenireServer.Presentation.Extensions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace InvenireServer.Presentation.Controllers.Commands;

[ApiController]
public class AdminCommandController : ControllerBase
{
    private readonly IConfiguration _configuration;
    private readonly IMediator _mediator;

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

        return Created($"/api/admin/{result.Admin.Id}", result.Token);
    }

    [EnableRateLimiting("SendVerificationPolicy")]
    [Authorize(Policy = Jwt.Policies.UNVERIFIED_ADMIN)]
    [HttpPost("/api/admins/email-verification/send")]
    public async Task<IActionResult> SendVerification()
    {
        await _mediator.Send(new SendVerificationAdminCommand
        {
            Jwt = JwtBuilder.Parse(HttpContext.Request.Headers.ParseBearerToken()),
            FrontendBaseUrl = _configuration.GetSection("Frontend:BaseUrl").Value ?? throw new NullReferenceException()
        });

        return NoContent();
    }

    [Authorize(Policy = Jwt.Policies.UNVERIFIED_ADMIN)]
    [HttpPost("/api/admins/email-verification/confirm")]
    public async Task<IActionResult> ConfirmVerification()
    {
        await _mediator.Send(new ConfirmVerificationAdminCommand
        {
            Jwt = JwtBuilder.Parse(HttpContext.Request.Headers.ParseBearerToken())
        });

        return NoContent();
    }

    [EnableRateLimiting("LoginPolicy")]
    [HttpPost("/api/admins/login")]
    public async Task<IActionResult> Login([FromBody] LoginAdminCommand command)
    {
        if (command is null)
            throw new ValidationException([new ValidationFailure("", "Request body is missing or invalid.")]);

        var result = await _mediator.Send(command);

        return Ok(result.Token);
    }

    [Authorize(Policy = Jwt.Policies.ADMIN)]
    [HttpPut("/api/admins")]
    public async Task<IActionResult> Update([FromBody] UpdateAdminCommand command)
    {
        if (command is null)
            throw new ValidationException([new ValidationFailure("", "Request body is missing or invalid.")]);

        command = command with
        {
            Jwt = JwtBuilder.Parse(HttpContext.Request.Headers.ParseBearerToken())
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
            Jwt = JwtBuilder.Parse(HttpContext.Request.Headers.ParseBearerToken())
        });

        return NoContent();
    }
}