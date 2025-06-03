using InvenireServer.Application.Dtos.Employees;
using InvenireServer.Application.Interfaces.Common;
using InvenireServer.Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace InvenireServer.Application.Mappers;

/// <summary>
/// Maps a <see cref="RegisterEmployeeDto"/> to an <see cref="Employee"/> entity, including password hashing.
/// </summary>
public class EmployeeMapper : IMapper<Employee, RegisterEmployeeDto>
{
    private readonly IPasswordHasher<Employee> _hasher;

    /// <summary>
    /// Initializes a new instance of the <see cref="EmployeeMapper"/> class using the provided password hasher.
    /// </summary>
    /// <param name="hasher">The password hasher used to securely hash employee passwords.</param>
    public EmployeeMapper(IPasswordHasher<Employee> hasher)
    {
        _hasher = hasher;
    }

    /// <summary>
    /// Maps the specified DTO to a new <see cref="Employee"/> entity and hashes the provided password.
    /// </summary>
    /// <param name="dto">The DTO containing registration information for the employee.</param>
    /// <returns>A new <see cref="Employee"/> instance with populated fields and a hashed password.</returns>
    public Employee Map(RegisterEmployeeDto dto)
    {
        // Map the object.
        var employee = new Employee
        {
            Id = dto.Id ?? Guid.NewGuid(),
            Name = dto.Name,
            EmailAddress = dto.EmailAddress,
            IsVerified = false,
            Password = dto.Password,
            UpdatedAt = null,
            CreatedAt = DateTimeOffset.Now,
            LastLoginAt = null,
        };

        // Hash the password.
        employee.Password = _hasher.HashPassword(employee, employee.Password);

        return employee;
    }
}