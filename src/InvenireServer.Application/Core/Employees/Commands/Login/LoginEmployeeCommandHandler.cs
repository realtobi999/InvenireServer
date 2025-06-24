using System;
using System.Security.Claims;
using InvenireServer.Application.Interfaces.Managers;
using InvenireServer.Domain.Entities.Common;
using InvenireServer.Domain.Entities.Users;
using InvenireServer.Domain.Exceptions.Http;
using Microsoft.AspNetCore.Identity;

namespace InvenireServer.Application.Core.Employees.Commands.Login;

public class LoginEmployeeCommandHandler : IRequestHandler<LoginEmployeeCommand, LoginEmployeeCommandResult>
{
    private readonly IJwtManager _jwt;
    private readonly IServiceManager _services;
    private readonly IPasswordHasher<Employee> _hasher;

    public LoginEmployeeCommandHandler(IServiceManager services, IPasswordHasher<Employee> hasher, IJwtManager jwt)
    {
        _jwt = jwt;
        _hasher = hasher;
        _services = services;
    }

    public async Task<LoginEmployeeCommandResult> Handle(LoginEmployeeCommand request, CancellationToken cancellationToken)
    {
        // Retrieves the employee and returns 401 unauthorized if the admin is not found.
        Employee employee;
        try
        {
            employee = await _services.Employees.GetAsync(e => e.EmailAddress == request.EmailAddress);
        }
        catch (NotFound404Exception)
        {
            throw new Unauthorized401Exception("Invalid credentials.");
        }

        // Make sure that the employee is verified before logging in.
        if (!employee.IsVerified) throw new Unauthorized401Exception("Verification required to proceed.");

        // Validate the provided credentials.
        if (_hasher.VerifyHashedPassword(employee, employee.Password, request.Password) == PasswordVerificationResult.Failed) throw new Unauthorized401Exception("Invalid credentials.");

        // Update the timestamp of the employee's last login.
        employee.LastLoginAt = DateTimeOffset.Now;
        await _services.Employees.UpdateAsync(employee);

        // Create the jwt.
        var jwt = _jwt.Builder.Build([
            new Claim("role", Jwt.Roles.EMPLOYEE),
            new Claim("employee_id", employee.Id.ToString()),
            new Claim("is_verified", bool.TrueString)
        ]);

        return new LoginEmployeeCommandResult
        {
            Token = _jwt.Writer.Write(jwt)
        };
    }
}
