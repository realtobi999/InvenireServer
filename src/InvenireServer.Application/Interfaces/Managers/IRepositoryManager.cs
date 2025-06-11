using InvenireServer.Domain.Interfaces.Repositories;

namespace InvenireServer.Application.Interfaces.Managers;

public interface IRepositoryManager
{
    IAdminRepository Admins { get; }

    IEmployeeRepository Employees { get; }

    IOrganizationRepository Organizations { get; }

    Task<int> SaveAsync();

    Task SaveOrThrowAsync();
}