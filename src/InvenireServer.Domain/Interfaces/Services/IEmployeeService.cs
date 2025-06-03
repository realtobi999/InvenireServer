using System.Linq.Expressions;
using InvenireServer.Domain.Entities;
using InvenireServer.Domain.Entities.Common;

namespace InvenireServer.Domain.Interfaces.Services;

/// <summary>
/// Defines contract for business logic operations for managing employees.
/// </summary>
public interface IEmployeeService
{
    /// <summary>
    /// Retrieves an employee based on the provided JWT token.
    /// </summary>
    /// <param name="jwt">The JWT token containing employee claims.</param>
    /// <returns>The employee associated with the token.</returns>
    Task<Employee> GetAsync(Jwt jwt);

    /// <summary>
    /// Retrieves an employee matching the specified predicate.
    /// </summary>
    /// <param name="predicate">A filter expression for querying employees.</param>
    /// <returns>The matching employee.</returns>
    Task<Employee> GetAsync(Expression<Func<Employee, bool>> predicate);

    /// <summary>
    /// Creates a new employee record.
    /// </summary>
    /// <param name="employee">The employee entity to create.</param>
    Task CreateAsync(Employee employee);

    /// <summary>
    /// Creates a new JWT for the specified employee.
    /// </summary>
    /// <param name="employee">The employee to generate a token for.</param>
    /// <returns>A JWT containing the employee's claims.</returns>
    Jwt CreateJwt(Employee employee);

    /// <summary>
    /// Updates an existing employee record.
    /// </summary>
    /// <param name="employee">The employee entity with updated data.</param>
    Task UpdateAsync(Employee employee);

    /// <summary>
    /// Sends an email verification message to the specified employee.
    /// </summary>
    /// <param name="employee">The employee to send verification to.</param>
    Task SendEmailVerificationAsync(Employee employee);

    /// <summary>
    /// Confirms the employee's email verification status.
    /// </summary>
    /// <param name="employee">The employee whose email is being confirmed.</param>
    Task ConfirmEmailVerificationAsync(Employee employee);
}
