using InvenireServer.Domain.Interfaces.Repositories;

namespace InvenireServer.Application.Interfaces.Managers;

/// <summary>
/// Manages access to repository interfaces and manages saving changes to the data store.
/// </summary>
public interface IRepositoryManager
{
    /// <summary>
    /// Gets the repository for employee-related data operations.
    /// </summary>
    IEmployeeRepository Employees { get; }

    /// <summary>
    /// Saves all pending changes to the database and returns the number of affected entries.
    /// </summary>
    /// <returns>The number of entries affected by the save operation.</returns>
    Task<int> SaveAsync();

    /// <summary>
    /// Saves all pending changes to the database.
    /// Throws an exception if no entries were affected by the save operation.
    /// </summary>
    Task SaveOrThrowAsync();
}