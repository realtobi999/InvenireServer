using InvenireServer.Domain.Core.Entities;
using InvenireServer.Domain.Core.Exceptions.Http;
using InvenireServer.Domain.Core.Interfaces.Common;
using InvenireServer.Domain.Core.Interfaces.Managers;

namespace InvenireServer.Application.Core.Validators;

public class EmployeeValidator : IValidator<Employee>
{
    private readonly IRepositoryManager _repositories;

    public EmployeeValidator(IRepositoryManager repositories)
    {
        _repositories = repositories;
    }

    public async Task<(bool isValid, Exception? exception)> ValidateAsync(Employee employee)
    {
        // Ensure that the EmailAddress is unique.
        if (!await _repositories.Employee.IsEmailAddressUnique(employee.EmailAddress))
        {
            return (false, new BadRequest400Exception("Invalid value for EmailAddress: the address is already in use."));
        }

        // Ensure that the LoginAttempts is between correct bounds.
        if (employee.LoginAttempts < 0 || employee.LoginAttempts > Employee.MAX_LOGIN_ATTEMPTS)
        {
            return (false, new BadRequest400Exception("Invalid value for LoginAttempts: must be between 0 and the maximum allowed."));
        }

        // Ensure that if LoginLock.IsSet is true then LoginLock.ExpirationDate must not be null and must be in the future.
        if (employee.LoginLock.IsSet)
        {
            if (employee.LoginLock.ExpirationDate is null)
            {
                return (false, new BadRequest400Exception("Invalid value for LoginLock.ExpirationDate: must be provided when the lock is set."));
            }
            if (employee.LoginLock.ExpirationDate < DateTimeOffset.UtcNow)
            {
                return (false, new BadRequest400Exception("Invalid value for LoginLock.ExpirationDate: must be set in the future."));
            }
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

        // TODO: add validation to make sure that the assigned organization exits.

        return (true, null);
    }
}
