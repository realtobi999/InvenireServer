namespace InvenireServer.Application.Interfaces.Services.Employees;

/// <summary>
/// Defines a cleanup service for inactive employees.
/// </summary>
public interface IEmployeeCleanupService
{
    /// <summary>
    /// Performs cleanup of inactive employees.
    /// </summary>
    /// <returns>Awaitable task representing the cleanup operation.</returns>
    Task CleanupAsync();
}
