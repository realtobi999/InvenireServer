using InvenireServer.Application.Dtos.Employees;
using InvenireServer.Application.Interfaces.Factories.Users;
using InvenireServer.Domain.Entities.Users;
using Microsoft.AspNetCore.Identity;

namespace InvenireServer.Application.Factories.Users;

public class EmployeeFactory : IEmployeeFactory
{
    public Employee Create(RegisterEmployeeDto dto)
    {
        // Map the object.
        var employee = new Employee
        {
            Id = dto.Id ?? Guid.NewGuid(),
            Name = dto.Name,
            EmailAddress = dto.EmailAddress,
            Password = dto.Password,
            IsVerified = false,
            CreatedAt = DateTimeOffset.Now,
            LastUpdatedAt = null,
            LastLoginAt = null
        };

        // Hash the password.
        employee.Password = new PasswordHasher<Employee>().HashPassword(employee, employee.Password);

        return employee;
    }
}