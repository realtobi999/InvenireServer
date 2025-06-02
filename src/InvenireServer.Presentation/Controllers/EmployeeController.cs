using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.AspNetCore.Authorization;
using InvenireServer.Domain.Core.Entities;
using InvenireServer.Presentation.Extensions;
using InvenireServer.Domain.Core.Dtos.Employees;
using InvenireServer.Domain.Core.Exceptions.Http;
using InvenireServer.Domain.Core.Interfaces.Common;
using InvenireServer.Domain.Core.Interfaces.Managers;
using InvenireServer.Domain.Core.Entities.Common;
using InvenireServer.Application.Core.Authentication;

namespace InvenireServer.Presentation.Controllers;

/// <summary>
/// Handles HTTP requests related to employee actions.
/// </summary>
[ApiController]
public class EmployeeController : ControllerBase
{
    private readonly IJwtManager _jwt;
    private readonly IServiceManager _services;
    private readonly IMapper<Employee, RegisterEmployeeDto> _mapper;

    /// <summary>
    /// Initializes a new instance of the <see cref="EmployeeController"/> class.
    /// </summary>
    /// <param name="services">Service manager providing business logic access.</param>
    /// <param name="jwt">JWT manager for token handling.</param>
    /// <param name="factories">Factory manager for creating mappers and other dependencies.</param>
    public EmployeeController(IServiceManager services, IJwtManager jwt, IFactoryManager factories)
    {
        _jwt = jwt;
        _mapper = factories.Mappers.Initiate<Employee, RegisterEmployeeDto>();
        _services = services;
    }

    /// <summary>
    /// Registers a new employee account.
    /// </summary>
    /// <param name="dto">Registration data transfer object containing employee details.</param>
    /// <returns>Returns a Created response with the location of the new employee resource.</returns>
    [HttpPost("/api/auth/employee/register")]
    public async Task<IActionResult> RegisterEmployee(RegisterEmployeeDto dto)
    {
        var employee = _mapper.Map(dto);

        await _services.Employees.CreateAsync(employee);

        return Created($"/api/employee/{employee.Id}", null);
    }

    /// <summary>
    /// Authenticates an employee and returns a JWT on successful login.
    /// </summary>
    /// <param name="dto">Login data transfer object containing email and password.</param>
    /// <returns>Returns an OK response with a JWT token if authentication is successful.</returns>
    [EnableRateLimiting("LoginPolicy")]
    [HttpPost("/api/auth/employee/login")]
    public async Task<IActionResult> LoginEmployee(LoginEmployeeDto dto)
    {
        var employee = await _services.Employees.GetAsync(e => e.EmailAddress == dto.EmailAddress);

        var hasher = new PasswordHasher<Employee>();
        if (hasher.VerifyHashedPassword(employee, employee.Password, dto.Password) == PasswordVerificationResult.Failed)
        {
            throw new NotAuthorized401Exception();
        }

        var jwt = _services.Employees.CreateJwt(employee);

        return Ok(new LoginEmployeeResponseDto
        {
            Token = _jwt.Writer.Write(jwt)
        });
    }

    /// <summary>
    /// Sends an email verification link to the authenticated employee.
    /// </summary>
    /// <returns>Returns a NoContent response after the email has been sent.</returns>
    [Authorize(Policy = Jwt.Policies.UNVERIFIED_EMPLOYEE)]
    [HttpPost("/api/auth/employee/email-verification/send")]
    public async Task<IActionResult> SendEmailVerification()
    {
        var jwt = JwtBuilder.Parse(HttpContext.Request.Headers.ParseBearerToken());

        var employee = await _services.Employees.GetAsync(jwt);
        await _services.Employees.SendEmailVerificationAsync(employee);

        return NoContent();
    }

    /// <summary>
    /// Confirms the email verification for the authenticated employee based on the JWT.
    /// </summary>
    /// <returns>Returns a NoContent response after the email verification status is updated.</returns>
    [Authorize(Policy = Jwt.Policies.UNVERIFIED_EMPLOYEE)]
    [HttpPost("/api/auth/employee/email-verification/confirm")]
    public async Task<IActionResult> ConfirmEmailVerification()
    {
        var jwt = JwtBuilder.Parse(HttpContext.Request.Headers.ParseBearerToken());
        if (!jwt.Payload.Any(c => c.Type == "purpose" && c.Value == "email_verification"))
        {
            throw new NotAuthorized401Exception("Indication that the token is used for email verification is missing.");
        }

        var employee = await _services.Employees.GetAsync(jwt);
        await _services.Employees.ConfirmEmailVerificationAsync(employee);

        return NoContent();
    }
}
