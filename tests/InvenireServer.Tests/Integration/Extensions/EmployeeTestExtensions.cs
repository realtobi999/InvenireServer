using InvenireServer.Application.Dtos.Employees;
using InvenireServer.Domain.Entities;

namespace InvenireServer.Tests.Integration.Extensions;

/// <summary>
/// Provides extension methods for the <see cref="Employee"/> class to support test scenarios.
/// </summary>
public static class EmployeeTestExtensions
{
    /// <summary>
    /// Converts an <see cref="Employee"/> instance to a <see cref="RegisterEmployeeDto"/> with matching property values.
    /// </summary>
    /// <param name="employee">The employee entity to convert.</param>
    /// <returns>A new <see cref="RegisterEmployeeDto"/> instance populated with the employee's data.</returns>
    public static RegisterEmployeeDto ToRegisterEmployeeDto(this Employee employee)
    {
        var dto = new RegisterEmployeeDto
        {
            Id = employee.Id,
            Name = employee.Name,
            EmailAddress = employee.EmailAddress,
            Password = employee.Password,
            PasswordConfirm = employee.Password
        };

        return dto;
    }
}
