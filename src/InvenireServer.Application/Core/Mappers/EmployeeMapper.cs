using Microsoft.AspNetCore.Identity;
using InvenireServer.Domain.Core.Entities;
using InvenireServer.Domain.Core.Dtos.Employees;
using InvenireServer.Domain.Core.Interfaces.Common;

namespace InvenireServer.Application.Core.Mappers;

public class EmployeeMapper : IMapper<Employee, RegisterEmployeeDto>
{
    private readonly IPasswordHasher<Employee> _hasher;

    public EmployeeMapper(IPasswordHasher<Employee> hasher)
    {
        _hasher = hasher;
    }

    public Employee Map(RegisterEmployeeDto dto)
    {
        // Map the object.
        var employee = new Employee
        {
            Id = dto.Id ?? Guid.NewGuid(),
            Name = dto.Name,
            EmailAddress = dto.EmailAddress,
            Password = dto.Password,
            UpdatedAt = null,
            CreatedAt = DateTimeOffset.Now,
        };

        // Hash the password.
        employee.Password = _hasher.HashPassword(employee, employee.Password);

        return employee;
    }
}
