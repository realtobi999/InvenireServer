namespace InvenireServer.Domain.Core.Interfaces.Managers;

public interface IRepositoryManager
{
    /// <summary>
    /// Saves all changes to the database and returns the number of affected entries.
    /// <returns> The number of affected entries.</returns>
    /// </summary>
    Task<int> SaveAsync();

    /// <summary>
    /// Saves all changes to the database. Throws an exception if no entries were affected.
    /// </summary>
    Task SaveOrThrowAsync();
}