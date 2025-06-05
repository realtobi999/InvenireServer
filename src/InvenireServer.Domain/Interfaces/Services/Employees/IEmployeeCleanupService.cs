namespace InvenireServer.Domain.Interfaces.Services.Employees;

/// <summary>
/// Defines contract for a service that cleans up unverified employee accounts.
/// </summary>
public interface IEmployeeCleanupService
{
    /// <summary>
    /// Removes unverified employee accounts that meet the cleanup criteria.
    /// </summary>
    /// <returns>A task representing the asynchronous cleanup operation.</returns>
    Task CleanupAsync();
}