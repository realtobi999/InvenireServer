using InvenireServer.Application.Interfaces.Common;
using InvenireServer.Application.Interfaces.Managers;
using InvenireServer.Domain.Entities;
using InvenireServer.Domain.Exceptions.Http;

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
        // Ensure that the EmailAddress is unique.
        if (!await _repositories.Employees.HasUniqueEmailAddressAsync(employee))
        {
            return (false, new BadRequest400Exception("Invalid value for EmailAddress: the address is already in use."));
        }

        // Ensure that if UpdatedAt is set it must be after CreatedAt.
        if (employee.UpdatedAt is not null && employee.CreatedAt >= employee.UpdatedAt)
        {
            return (false, new BadRequest400Exception("Invalid value for UpdatedAt: must be greater than CreatedAt."));
        }

        // Ensure that CreatedAt is not set in the future.
        if (employee.CreatedAt > DateTimeOffset.UtcNow)
        {
            return (false, new BadRequest400Exception("Invalid value for CreatedAt: cannot be set in the future."));
        }

        // TODO: add validation to make sure that the assigned organization exists.

        return (true, null);
    }
}