using InvenireServer.Domain.Interfaces.Repositories.Admins;
using InvenireServer.Domain.Interfaces.Repositories.Employees;
using InvenireServer.Domain.Interfaces.Repositories.Organizations;

namespace InvenireServer.Application.Interfaces.Managers;

public interface IRepositoryManager
{
    IAdminRepository Admins { get; }

    IEmployeeRepository Employees { get; }

    IOrganizationRepository Organizations { get; }

    IOrganizationInvitationRepository OrganizationInvitations { get; }

    Task<int> SaveAsync();

    Task SaveOrThrowAsync();
}