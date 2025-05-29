using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.AspNetCore.Authorization;
using InvenireServer.Domain.Core.Entities;
using InvenireServer.Presentation.Extensions;
using InvenireServer.Application.Core.Factories;
using InvenireServer.Domain.Core.Dtos.Employees;
using InvenireServer.Domain.Core.Exceptions.Http;
using InvenireServer.Domain.Core.Entities.Common;
using InvenireServer.Domain.Core.Interfaces.Common;
using InvenireServer.Domain.Core.Interfaces.Managers;
using InvenireServer.Domain.Core.Interfaces.Factories;

namespace InvenireServer.Presentation.Controllers;

[ApiController]
public class EmployeeController : ControllerBase
{
    private readonly IJwtFactory _jwt;
    private readonly IServiceManager _services;
    private readonly IPasswordHasher<Employee> _hasher;
    private readonly IMapper<Employee, RegisterEmployeeDto> _mapper;

    public EmployeeController(IServiceManager services, IPasswordHasher<Employee> hasher, IFactoryManager factories)
    {
        _jwt = factories.Jwt;
        _mapper = factories.Mappers.Initiate<Employee, RegisterEmployeeDto>();
        _hasher = hasher;
        _services = services;
    }

    [HttpPost("/api/auth/employee/register")]
    public async Task<IActionResult> RegisterEmployee(RegisterEmployeeDto dto)
    {
        var employee = _mapper.Map(dto);

        await _services.Employees.CreateAsync(employee);

        return Created($"/api/employee/{employee.Id}", null);
    }

    [EnableRateLimiting("LoginPolicy")]
    [HttpPost("/api/auth/employee/login")]
    public async Task<IActionResult> LoginEmployee(LoginEmployeeDto dto)
    {
        try
        {
            var employee = await _services.Employees.GetAsync(e => e.EmailAddress == dto.EmailAddress);

            if (_hasher.VerifyHashedPassword(employee, employee.Password, dto.Password) == PasswordVerificationResult.Failed)
            {
                throw new NotAuthorized401Exception("Invalid email or password.");
            }

            var jwt = _jwt.Create([
                new("role", JwtFactory.Policies.Employee),
                new("employee_id", employee.Id.ToString())
            ]);

            return Ok(new LoginEmployeeResponseDto
            {
                Token = jwt.Write()
            });
        }
        catch (NotFound404Exception)
        {
            throw new NotAuthorized401Exception("Invalid email or password.");
        }
    }

    [Authorize(Policy = JwtFactory.Policies.Employee)]
    [HttpPost("/api/auth/employee/send-email-verification")]
    public async Task<IActionResult> SendVerificationEmail()
    {
        var jwt = Jwt.Parse(HttpContext.Request.Headers.ParseBearerToken());

        var employee = await _services.Employees.GetAsync(jwt);
        await _services.Employees.SendVerificationEmailAsync(employee, HttpContext.Request);

        return NoContent();
    }
}
