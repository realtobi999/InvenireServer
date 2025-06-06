using InvenireServer.Application.Dtos.Admins;
using InvenireServer.Application.Interfaces.Factories.Admins;
using InvenireServer.Application.Interfaces.Managers;
using InvenireServer.Domain.Entities.Common;
using InvenireServer.Domain.Exceptions.Http;
using InvenireServer.Infrastructure.Authentication;
using InvenireServer.Presentation.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace InvenireServer.Presentation.Controllers;

[ApiController]
public class AdminController : ControllerBase
{
    private readonly IAdminFactory _factory;
    private readonly IServiceManager _services;

    public AdminController(IServiceManager services, IFactoryManager factories)
    {
        _factory = factories.Entities.Admins;
        _services = services;
    }

    [HttpPost("/api/auth/admin/register")]
    public async Task<IActionResult> RegisterAdmin(RegisterAdminDto dto)
    {
        var admin = _factory.Create(dto);

        await _services.Admins.CreateAsync(admin);

        return Created($"/api/admin/{admin.Id}", null);
    }

    [EnableRateLimiting("SendEmailVerificationPolicy")]
    [Authorize(Policy = Jwt.Policies.UNVERIFIED_ADMIN)]
    [HttpPost("/api/auth/admin/email-verification/send")]
    public async Task<IActionResult> SendEmailVerification()
    {
        var jwt = JwtBuilder.Parse(HttpContext.Request.Headers.ParseBearerToken());

        var admin = await _services.Admins.GetAsync(jwt);
        await _services.Admins.SendEmailVerificationAsync(admin);

        return NoContent();
    }

    [Authorize(Policy = Jwt.Policies.UNVERIFIED_ADMIN)]
    [HttpPost("/api/auth/admin/email-verification/confirm")]
    public async Task<IActionResult> ConfirmEmailVerification()
    {
        var jwt = JwtBuilder.Parse(HttpContext.Request.Headers.ParseBearerToken());
        if (!jwt.Payload.Any(c => c.Type == "purpose" && c.Value == "email_verification"))
        {
            throw new Unauthorized401Exception("Indication that the token is used for email verification is missing.");
        }

        var admin = await _services.Admins.GetAsync(jwt);
        await _services.Admins.ConfirmEmailVerificationAsync(admin);

        return NoContent();
    }
}
