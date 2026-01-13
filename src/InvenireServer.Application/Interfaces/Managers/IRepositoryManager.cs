using InvenireServer.Application.Interfaces.Repositories.Organizations;
using InvenireServer.Application.Interfaces.Repositories.Properties;
using InvenireServer.Application.Interfaces.Repositories.Users;

namespace InvenireServer.Application.Interfaces.Managers;

/// <summary>
/// Defines a manager for repositories and persistence operations.
/// </summary>
public interface IRepositoryManager
{
    /// <summary>
    /// Repository for admin entities.
    /// </summary>
    IAdminRepository Admins { get; }

    /// <summary>
    /// Repository for employee entities.
    /// </summary>
    IEmployeeRepository Employees { get; }

    /// <summary>
    /// Repository for property entities.
    /// </summary>
    IPropertyRepository Properties { get; }

    /// <summary>
    /// Repository for organization entities.
    /// </summary>
    IOrganizationRepository Organizations { get; }

    /// <summary>
    /// Persists pending changes to the data store.
    /// </summary>
    /// <returns>Number of affected rows.</returns>
    Task<int> SaveAsync();

    /// <summary>
    /// Persists pending changes to the data store and throws an exception if none were saved.
    /// </summary>
    /// <returns>Awaitable task representing the save operation.</returns>
    Task SaveOrThrowAsync();
}
