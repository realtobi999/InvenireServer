using InvenireServer.Application.Core.Employees.Commands.Register;
using InvenireServer.Domain.Entities.Users;
using InvenireServer.Infrastructure.Persistence;

namespace InvenireServer.Tests.Extensions.Users;

/// <summary>
/// Provides test extensions for <see cref="Employee"/>.
/// </summary>
public static class EmployeeTestExtensions
{
    /// <summary>
    /// Creates a <see cref="RegisterEmployeeCommand"/> from an employee instance.
    /// </summary>
    /// <param name="employee">Source employee.</param>
    /// <returns>Register employee command.</returns>
    public static RegisterEmployeeCommand ToRegisterEmployeeCommand(this Employee employee)
    {
        var dto = new RegisterEmployeeCommand
        {
            Id = employee.Id,
            FirstName = employee.FirstName,
            LastName = employee.LastName,
            EmailAddress = employee.EmailAddress,
            Password = employee.Password,
            PasswordConfirm = employee.Password
        };

        return dto;
    }

    /// <summary>
    /// Marks the employee as verified in the database.
    /// </summary>
    /// <param name="employee">Employee to verify.</param>
    /// <param name="context">Database context to update.</param>
    public static void SetAsVerified(this Employee employee, InvenireServerContext context)
    {
        context.Employees.FirstOrDefault(e => e.Id == employee.Id)!.Verify();
        context.SaveChanges();
    }
}
