using InvenireServer.Application.Dtos.Employees;
using InvenireServer.Application.Interfaces.Factories.Employees;
using InvenireServer.Application.Interfaces.Managers;
using InvenireServer.Domain.Entities;
using InvenireServer.Domain.Entities.Common;
using InvenireServer.Domain.Exceptions.Http;
using InvenireServer.Infrastructure.Authentication;
using InvenireServer.Presentation.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace InvenireServer.Presentation.Controllers;

[ApiController]
public class EmployeeController : ControllerBase
{
    private readonly IEmployeeFactory _factory;
    private readonly IJwtManager _jwt;
    private readonly IServiceManager _services;

    public EmployeeController(IServiceManager services, IJwtManager jwt, IFactoryManager factories)
    {
        _jwt = jwt;
        _factory = factories.Entities.Employees;
        _services = services;
    }

    [HttpPost("/api/auth/employee/register")]
    public async Task<IActionResult> RegisterEmployee(RegisterEmployeeDto dto)
    {
        var employee = _factory.Create(dto);

        await _services.Employees.CreateAsync(employee);

        return Created($"/api/employee/{employee.Id}", null);
    }

    [EnableRateLimiting("SendEmailVerificationPolicy")]
    [Authorize(Policy = Jwt.Policies.UNVERIFIED_EMPLOYEE)]
    [HttpPost("/api/auth/employee/email-verification/send")]
    public async Task<IActionResult> SendEmailVerification()
    {
        var jwt = JwtBuilder.Parse(HttpContext.Request.Headers.ParseBearerToken());

        var employee = await _services.Employees.GetAsync(jwt);
        await _services.Employees.SendEmailVerificationAsync(employee);

        return NoContent();
    }

    [Authorize(Policy = Jwt.Policies.UNVERIFIED_EMPLOYEE)]
    [HttpPost("/api/auth/employee/email-verification/confirm")]
    public async Task<IActionResult> ConfirmEmailVerification()
    {
        var jwt = JwtBuilder.Parse(HttpContext.Request.Headers.ParseBearerToken());
        if (!jwt.Payload.Any(c => c.Type == "purpose" && c.Value == "email_verification")) throw new Unauthorized401Exception("Indication that the token is used for email verification is missing.");

        var employee = await _services.Employees.GetAsync(jwt);
        await _services.Employees.ConfirmEmailVerificationAsync(employee);

        return NoContent();
    }

    [EnableRateLimiting("LoginPolicy")]
    [HttpPost("/api/auth/employee/login")]
    public async Task<IActionResult> LoginEmployee(LoginEmployeeDto dto)
    {
        // Retrieves the employee and returns 401 unauthorized if the user is not found.
        Employee employee;
        try
        {
            employee = await _services.Employees.GetAsync(e => e.EmailAddress == dto.EmailAddress);
        }
        catch (NotFound404Exception)
        {
            throw new Unauthorized401Exception("Invalid credentials.");
        }

        // Make sure that the employee is verified before logging in.
        if (!employee.IsVerified) throw new Unauthorized401Exception("Email verification required to proceed.");

        // Validate the provided credentials.
        var hasher = new PasswordHasher<Employee>();
        if (hasher.VerifyHashedPassword(employee, employee.Password, dto.Password) == PasswordVerificationResult.Failed) throw new Unauthorized401Exception("Invalid credentials.");

        // Updates the timestamp of the user's last login.
        employee.LastLoginAt = DateTimeOffset.Now;
        await _services.Employees.UpdateAsync(employee);

        var jwt = _services.Employees.CreateJwt(employee);
        return Ok(new LoginEmployeeResponseDto
        {
            Token = _jwt.Writer.Write(jwt)
        });
    }
}