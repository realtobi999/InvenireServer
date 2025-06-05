using Microsoft.AspNetCore.Identity;
using InvenireServer.Domain.Entities;
using InvenireServer.Application.Dtos.Employees;
using InvenireServer.Application.Interfaces.Factories.Employees;

namespace InvenireServer.Application.Factories.Employees;

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
            LastLoginAt = null,
        };

        // Hash the password.
        employee.Password = new PasswordHasher<Employee>().HashPassword(employee, employee.Password);

        return employee;
    }
}
