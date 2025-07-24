using InvenireServer.Application.Interfaces.Repositories.Organizations;
using InvenireServer.Application.Interfaces.Repositories.Properties;
using InvenireServer.Application.Interfaces.Repositories.Users;

namespace InvenireServer.Application.Interfaces.Managers;

public interface IRepositoryManager
{
    IAdminRepository Admins { get; }

    IEmployeeRepository Employees { get; }

    IPropertyRepository Properties { get; }

    IOrganizationRepository Organizations { get; }

    Task<int> SaveAsync();

    Task SaveOrThrowAsync();
}