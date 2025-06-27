using InvenireServer.Application.Interfaces.Common;
using InvenireServer.Application.Interfaces.Managers;
using InvenireServer.Domain.Entities.Organizations;
using InvenireServer.Domain.Entities.Users;
using InvenireServer.Domain.Exceptions.Http;

namespace InvenireServer.Application.Validators.Organizations;

public class OrganizationValidator : IValidator<Organization>
{
    private readonly IRepositoryManager _repositories;

    public OrganizationValidator(IRepositoryManager repositories)
    {
        _repositories = repositories;
    }

    public async Task<(bool isValid, Exception? exception)> ValidateAsync(Organization organization)
    {
        // Name must be unique.
        if (await _repositories.Organizations.GetAsync(o => o.Name == organization.Name && o.Id != organization.Id) is not null) return (false, new BadRequest400Exception($"{nameof(Organization.Name)} must be unique among all organizations."));

        // Last update must be later than creation time, if set.
        if (organization.LastUpdatedAt is not null && organization.CreatedAt >= organization.LastUpdatedAt) return (false, new BadRequest400Exception($"{nameof(Organization.LastUpdatedAt)} must be later than CreatedAt."));

        // Creation time cannot be set in the future.
        if (organization.CreatedAt > DateTimeOffset.UtcNow) return (false, new BadRequest400Exception($"{nameof(Organization.CreatedAt)} cannot be set in the future."));

        // Admin must be assigned.
        if (organization.Admin is null) return (false, new BadRequest400Exception($"{nameof(Admin)} must be assigned."));

        // Admin must exist in the system.
        var admin = await _repositories.Admins.GetAsync(a => a.Id == organization.Admin.Id);
        if (admin is null) return (false, new NotFound404Exception($"The assigned {nameof(Admin)} was not found in the system."));

        // Employees if set, must exist in the system.
        if (organization.Employees?.Any() == true)
            foreach (var employee in organization.Employees)
                if (await _repositories.Employees.GetAsync(e => e.Id == employee.Id) is null)
                    return (false, new NotFound404Exception($"The assigned {nameof(Employee)} was not found in the system."));

        return (true, null);
    }
}