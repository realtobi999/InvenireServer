using InvenireServer.Application.Interfaces.Common;
using InvenireServer.Application.Interfaces.Managers;
using InvenireServer.Domain.Entities.Organizations;
using InvenireServer.Domain.Entities.Users;
using InvenireServer.Domain.Exceptions.Http;

namespace InvenireServer.Application.Validators.Organizations;

public class OrganizationInvitationValidator : IEntityValidator<OrganizationInvitation>
{
    private readonly IRepositoryManager _repositories;

    public OrganizationInvitationValidator(IRepositoryManager repositories)
    {
        _repositories = repositories;
    }

    public async Task<(bool isValid, Exception? exception)> ValidateAsync(OrganizationInvitation invitation)
    {
        // Last update must be later than creation time, if set.
        if (invitation.LastUpdatedAt is not null && invitation.CreatedAt >= invitation.LastUpdatedAt) return (false, new BadRequest400Exception($"{nameof(OrganizationInvitation.LastUpdatedAt)} must be later than CreatedAt."));

        // Creation time cannot be set in the future.
        if (invitation.CreatedAt > DateTimeOffset.UtcNow) return (false, new BadRequest400Exception($"{nameof(OrganizationInvitation.CreatedAt)} cannot be set in the future."));

        // Organization must be assigned.
        if (invitation.OrganizationId is null) return (false, new BadRequest400Exception($"{nameof(Organization)}) must be assigned."));

        // Organization must exist in the system
        var organization = await _repositories.Organizations.GetAsync(o => o.Id == invitation.OrganizationId);
        if (organization is null) return (false, new NotFound404Exception($"The assigned {nameof(Organization)} was not found in the system."));

        // Employee must be assigned.
        if (invitation.Employee is null) return (false, new BadRequest400Exception($"{nameof(Employee)} must be assigned."));

        // Employee must be exist in the system.
        var employee = await _repositories.Employees.GetAsync(e => e.Id == invitation.Employee.Id);
        if (employee is null) return (false, new NotFound404Exception($"The assigned {nameof(Employee)} was not found in the system."));

        return (true, null);
    }
}