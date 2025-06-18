using InvenireServer.Application.Interfaces.Common;
using InvenireServer.Application.Interfaces.Managers;
using InvenireServer.Domain.Entities.Organizations;
using InvenireServer.Domain.Entities.Users;
using InvenireServer.Domain.Exceptions.Http;

namespace InvenireServer.Application.Validators;

public class EmployeeValidator : IValidator<Employee>
{
    private readonly IRepositoryManager _repositories;

    public EmployeeValidator(IRepositoryManager repositories)
    {
        _repositories = repositories;
    }

    public async Task<(bool isValid, Exception? exception)> ValidateAsync(Employee employee)
    {
        // Email address must be unique.
        if (await _repositories.Employees.GetAsync(e => e.EmailAddress == employee.EmailAddress && e.Id != employee.Id) is not null) return (false, new BadRequest400Exception($"{nameof(Employee.EmailAddress)} must be unique among all employees."));

        // Last login must be later than creation time, if set.
        if (employee.LastLoginAt is not null && employee.CreatedAt >= employee.LastLoginAt) return (false, new BadRequest400Exception($"{nameof(Employee.LastLoginAt)} must be later than CreatedAt."));

        // Last update must be later than creation time, if set.
        if (employee.LastUpdatedAt is not null && employee.CreatedAt >= employee.LastUpdatedAt) return (false, new BadRequest400Exception($"{nameof(Employee.LastUpdatedAt)} must be later than CreatedAt."));

        // Creation time cannot be set in the future.
        if (employee.CreatedAt > DateTimeOffset.UtcNow) return (false, new BadRequest400Exception($"{nameof(Employee.CreatedAt)} cannot be set in the future."));

        // Organization if set, must exit.
        if (employee.OrganizationId is not null)
        {
            var organization = await _repositories.Organizations.GetAsync(o => o.Id == employee.OrganizationId);

            if (organization is null) return (false, new NotFound404Exception($"The assigned {nameof(Organization)} was not found in the system."));
        }

        return (true, null);
    }
}