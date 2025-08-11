using System.Security.Claims;
using InvenireServer.Application.Interfaces.Managers;
using InvenireServer.Domain.Entities.Common;
using InvenireServer.Domain.Entities.Users;
using Microsoft.AspNetCore.Identity;

namespace InvenireServer.Application.Core.Employees.Commands.Register;

public class RegisterEmployeeCommandHandler : IRequestHandler<RegisterEmployeeCommand, RegisterEmployeeCommandResult>
{
    private readonly IJwtManager _jwt;
    private readonly IRepositoryManager _repositories;
    private readonly IPasswordHasher<Employee> _hasher;

    public RegisterEmployeeCommandHandler(IJwtManager jwt, IPasswordHasher<Employee> hasher, IRepositoryManager repositories)
    {
        _jwt = jwt;
        _hasher = hasher;
        _repositories = repositories;
    }

    public async Task<RegisterEmployeeCommandResult> Handle(RegisterEmployeeCommand request, CancellationToken ct)
    {
        var employee = new Employee
        {
            Id = request.Id ?? Guid.NewGuid(),
            Name = request.Name,
            EmailAddress = request.EmailAddress,
            Password = request.Password,
            IsVerified = false,
            CreatedAt = DateTimeOffset.UtcNow,
            LastLoginAt = null,
            LastUpdatedAt = null
        };
        employee.Password = _hasher.HashPassword(employee, employee.Password);

        _repositories.Employees.Create(employee);

        await _repositories.SaveOrThrowAsync();

        return new RegisterEmployeeCommandResult
        {
            Employee = employee,
            Response = new RegisterEmployeeCommandResponse
            {
                Token = _jwt.Writer.Write(_jwt.Builder.Build([
                    new Claim("role", Jwt.Roles.EMPLOYEE),
                    new Claim("employee_id", employee.Id.ToString()),
                    new Claim("is_verified", bool.FalseString)
                ]))
            }
        };
    }
}