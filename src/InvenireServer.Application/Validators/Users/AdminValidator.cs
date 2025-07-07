using InvenireServer.Application.Interfaces.Common;
using InvenireServer.Application.Interfaces.Managers;
using InvenireServer.Domain.Entities.Organizations;
using InvenireServer.Domain.Entities.Users;
using InvenireServer.Domain.Exceptions.Http;

namespace InvenireServer.Application.Validators.Users;

public class AdminValidator : IEntityValidator<Admin>
{
    private readonly IRepositoryManager _repositories;

    public AdminValidator(IRepositoryManager repositories)
    {
        _repositories = repositories;
    }

    public async Task<(bool isValid, Exception? exception)> ValidateAsync(Admin admin)
    {
        // Email address must be unique.
        if (await _repositories.Admins.GetAsync(e => e.EmailAddress == admin.EmailAddress && e.Id != admin.Id) is not null) return (false, new BadRequest400Exception($"{nameof(Employee.EmailAddress)} must be unique among all employees."));

        // Last login must be later than creation time, if set.
        if (admin.LastLoginAt is not null && admin.CreatedAt >= admin.LastLoginAt) return (false, new BadRequest400Exception($"{nameof(Employee.LastLoginAt)} must be later than CreatedAt."));

        // Last update must be later than creation time, if set.
        if (admin.LastUpdatedAt is not null && admin.CreatedAt >= admin.LastUpdatedAt) return (false, new BadRequest400Exception($"{nameof(Employee.LastUpdatedAt)} must be later than CreatedAt."));

        // Creation time cannot be set in the future.
        if (admin.CreatedAt > DateTimeOffset.UtcNow) return (false, new BadRequest400Exception($"{nameof(Employee.CreatedAt)} cannot be set in the future."));

        // Organization if set, must exit.
        if (admin.OrganizationId is not null)
        {
            var organization = await _repositories.Organizations.GetAsync(o => o.Id == admin.OrganizationId);

            if (organization is null) return (false, new NotFound404Exception($"The assigned {nameof(Organization)} was not found in the system."));
        }

        return (true, null);
    }
}