using InvenireServer.Application.Interfaces.Common;
using InvenireServer.Domain.Interfaces.Repositories.Organizations;
using InvenireServer.Domain.Interfaces.Repositories.Properties;
using InvenireServer.Domain.Interfaces.Repositories.Users;

namespace InvenireServer.Application.Interfaces.Managers;

public interface IRepositoryManager
{
    IAdminRepository Admins { get; }

    IEmployeeRepository Employees { get; }

    IPropertyRepository Properties { get; }

    IOrganizationRepository Organizations { get; }

    Task<int> SaveAsync();

    Task SaveOrThrowAsync();

    Task<ITransaction> BeginTransactionAsync();
}