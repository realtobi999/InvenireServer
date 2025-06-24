using MediatR;
using InvenireServer.Application.Cqrs.Admins.Commands.Register;
using InvenireServer.Domain.Entities.Common;
using InvenireServer.Infrastructure.Authentication;
using InvenireServer.Presentation.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using InvenireServer.Application.Cqrs.Admins.Commands.Login;
using InvenireServer.Application.Cqrs.Admins.Commands.Verification.Confirm;
using InvenireServer.Application.Cqrs.Admins.Commands.Verification.Send;

namespace InvenireServer.Presentation.Controllers;

[ApiController]
public class AdminController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly IConfiguration _configuration;

    public AdminController(IMediator mediator, IConfiguration configuration)
    {
        _mediator = mediator;
        _configuration = configuration;
    }

    [HttpPost("/api/auth/admin/register")]
    public async Task<IActionResult> Register([FromBody] RegisterAdminCommand command)
    {
        var result = await _mediator.Send(command);

        return Created($"/api/admin/{result.Admin.Id}", result.Token);
    }

    [EnableRateLimiting("SendVerificationPolicy")]
    [Authorize(Policy = Jwt.Policies.UNVERIFIED_ADMIN)]
    [HttpPost("/api/auth/admin/email-verification/send")]
    public async Task<IActionResult> SendVerification()
    {
        var command = new SendVerificationAdminCommand
        {
            Jwt = JwtBuilder.Parse(HttpContext.Request.Headers.ParseBearerToken()),
            FrontendBaseUrl = _configuration.GetSection("Frontend:BaseUrl").Value ?? throw new NullReferenceException()
        };

        await _mediator.Send(command);

        return NoContent();
    }

    [Authorize(Policy = Jwt.Policies.UNVERIFIED_ADMIN)]
    [HttpPost("/api/auth/admin/email-verification/confirm")]
    public async Task<IActionResult> ConfirmVerification()
    {
        var command = new ConfirmVerificationAdminCommand
        {
            Jwt = JwtBuilder.Parse(HttpContext.Request.Headers.ParseBearerToken()),
        };

        await _mediator.Send(command);

        return NoContent();
    }

    [EnableRateLimiting("LoginPolicy")]
    [HttpPost("/api/auth/admin/login")]
    public async Task<IActionResult> Login(LoginAdminCommand command)
    {
        var result = await _mediator.Send(command);

        return Ok(result.Token);
    }
}