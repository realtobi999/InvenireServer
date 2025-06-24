
using System.Security.Claims;
using InvenireServer.Application.Interfaces.Managers;
using InvenireServer.Domain.Entities.Common;
using InvenireServer.Domain.Entities.Users;
using Microsoft.AspNetCore.Identity;

namespace InvenireServer.Application.Core.Employees.Commands.Register;

public class RegisterEmployeeCommandHandler : IRequestHandler<RegisterEmployeeCommand, RegisterEmployeeCommandResult>
{
    private readonly IJwtManager _jwt;
    private readonly IServiceManager _services;
    private readonly IPasswordHasher<Employee> _hasher;

    public RegisterEmployeeCommandHandler(IServiceManager services, IPasswordHasher<Employee> hasher, IJwtManager jwt)
    {
        _jwt = jwt;
        _hasher = hasher;
        _services = services;
    }

    public async Task<RegisterEmployeeCommandResult> Handle(RegisterEmployeeCommand request, CancellationToken _)
    {
        // Build the employee from the request and hash the password.
        var employee = new Employee
        {
            Id = request.Id ?? Guid.NewGuid(),
            Name = request.Name,
            EmailAddress = request.EmailAddress,
            Password = request.Password,
            IsVerified = false,
            CreatedAt = DateTimeOffset.UtcNow,
            LastLoginAt = null,
            LastUpdatedAt = null,
        };
        employee.Password = _hasher.HashPassword(employee, employee.Password);

        // Save the employee to the database.
        await _services.Employees.CreateAsync(employee);

        // Create the jwt.
        var jwt = _jwt.Builder.Build([
            new Claim("role", Jwt.Roles.EMPLOYEE),
            new Claim("admin_id", employee.Id.ToString()),
            new Claim("is_verified", bool.FalseString)
        ]);

        return new RegisterEmployeeCommandResult
        {
            Employee = employee,
            Token = _jwt.Writer.Write(jwt),
        };
    }
}
