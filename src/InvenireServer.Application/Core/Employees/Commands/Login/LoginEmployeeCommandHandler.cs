using System.Security.Claims;
using InvenireServer.Application.Interfaces.Managers;
using InvenireServer.Domain.Entities.Common;
using InvenireServer.Domain.Entities.Users;
using InvenireServer.Domain.Exceptions.Http;
using Microsoft.AspNetCore.Identity;

namespace InvenireServer.Application.Core.Employees.Commands.Login;

public class LoginEmployeeCommandHandler : IRequestHandler<LoginEmployeeCommand, LoginEmployeeCommandResult>
{
    private readonly IPasswordHasher<Employee> _hasher;
    private readonly IJwtManager _jwt;
    private readonly IServiceManager _services;

    public LoginEmployeeCommandHandler(IServiceManager services, IPasswordHasher<Employee> hasher, IJwtManager jwt)
    {
        _jwt = jwt;
        _hasher = hasher;
        _services = services;
    }

    public async Task<LoginEmployeeCommandResult> Handle(LoginEmployeeCommand request, CancellationToken ct)
    {
        // Verify the email belongs to  a  verified  employee  and  confirm  the
        // password; return unauthorized if any check fails.
        var employee = (Employee?)null;
        try
        {
            employee = await _services.Employees.GetAsync(e => e.EmailAddress == request.EmailAddress);
        }
        catch (NotFound404Exception)
        {
            throw new Unauthorized401Exception("Invalid credentials.");
        }
        if (!employee.IsVerified) throw new Unauthorized401Exception("Verification is required to proceed with login.");
        if (_hasher.VerifyHashedPassword(employee, employee.Password, request.Password) == PasswordVerificationResult.Failed) throw new Unauthorized401Exception("Invalid credentials.");

        employee.LastLoginAt = DateTimeOffset.UtcNow;

        await _services.Employees.UpdateAsync(employee);

        return new LoginEmployeeCommandResult
        {
            Token = _jwt.Writer.Write(_jwt.Builder.Build([
                new Claim("role", Jwt.Roles.EMPLOYEE),
                new Claim("employee_id", employee.Id.ToString()),
                new Claim("is_verified", bool.TrueString)
            ]))
        };
    }
}