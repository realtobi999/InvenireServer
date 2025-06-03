using InvenireServer.Domain.Entities;

namespace InvenireServer.Domain.Interfaces.Repositories;

/// <inheritdoc/>
public interface IEmployeeRepository : IBaseRepository<Employee>
{
    /// <summary>
    /// Checks whether the given employee's email address is unique within the repository.
    /// </summary>
    /// <param name="employee">The employee entity to check.</param>
    /// <returns>
    /// A task representing the asynchronous operation. The task result is <c>true</c> if the email is unique; otherwise, <c>false</c>.
    /// </returns>
    Task<bool> HasUniqueEmailAddressAsync(Employee employee);
}