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
    private readonly IRepositoryManager _repositories;
    private readonly IPasswordHasher<Employee> _hasher;

    public LoginEmployeeCommandHandler(IJwtManager jwt, IPasswordHasher<Employee> hasher, IRepositoryManager repositories)
    {
        _jwt = jwt;
        _hasher = hasher;
        _repositories = repositories;
    }

    public async Task<LoginEmployeeCommandResult> Handle(LoginEmployeeCommand request, CancellationToken ct)
    {
        var employee = await _repositories.Employees.GetAsync(e => e.EmailAddress == request.EmailAddress) ?? throw new Unauthorized401Exception("Invalid credentials.");

        if (!employee.IsVerified) throw new Unauthorized401Exception("Verification is required to proceed with login.");
        if (_hasher.VerifyHashedPassword(employee, employee.Password, request.Password) == PasswordVerificationResult.Failed) throw new Unauthorized401Exception("Invalid credentials.");

        employee.LastLoginAt = DateTimeOffset.UtcNow;

        await _repositories.Employees.ExecuteUpdateAsync(employee);

        var token = _jwt.Builder.Build(
            [
                new Claim("role", Jwt.Roles.EMPLOYEE),
                new Claim("employee_id", employee.Id.ToString()),
                new Claim("is_verified", bool.TrueString)
            ]);
        return new LoginEmployeeCommandResult
        {
            Token = token,
            TokenString = _jwt.Writer.Write(token)
        };
    }
}