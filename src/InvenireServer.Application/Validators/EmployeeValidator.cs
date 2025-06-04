using InvenireServer.Domain.Entities;
using InvenireServer.Domain.Exceptions.Http;
using InvenireServer.Application.Interfaces.Common;
using InvenireServer.Application.Interfaces.Managers;

namespace InvenireServer.Application.Validators;

/// <summary>
/// Provides validation logic specific to <see cref="Employee"/> entities.
/// </summary>
public class EmployeeValidator : IValidator<Employee>
{
    private readonly IRepositoryManager _repositories;

    /// <summary>
    /// Initializes a new instance of the <see cref="EmployeeValidator"/> class.
    /// </summary>
    /// <param name="repositories">Provides access to data repositories for validation checks.</param>
    public EmployeeValidator(IRepositoryManager repositories)
    {
        _repositories = repositories;
    }

    /// <inheritdoc/>
    public async Task<(bool isValid, Exception? exception)> ValidateAsync(Employee employee)
    {
        // Email address must be unique.
        if (await _repositories.Employees.GetAsync(e => e.EmailAddress == employee.EmailAddress && e.Id != employee.Id) is not null)
        {
            return (false, new BadRequest400Exception($"{nameof(Employee.EmailAddress)} must be unique among all employees."));
        }

        // Last login must be later than creation time, if set.
        if (employee.LastLoginAt is not null && employee.CreatedAt >= employee.LastLoginAt)
        {
            return (false, new BadRequest400Exception($"{nameof(Employee.LastLoginAt)} must be later than CreatedAt."));
        }

        // Last update must be later than creation time, if set.
        if (employee.LastUpdatedAt is not null && employee.CreatedAt >= employee.LastUpdatedAt)
        {
            return (false, new BadRequest400Exception($"{nameof(Employee.LastUpdatedAt)} must be later than CreatedAt."));
        }

        // Creation time cannot be set in the future.
        if (employee.CreatedAt > DateTimeOffset.UtcNow)
        {
            return (false, new BadRequest400Exception($"{nameof(Employee.CreatedAt)} cannot be set in the future."));
        }

        // TODO: Validate that the referenced organization exists.

        return (true, null);
    }
}