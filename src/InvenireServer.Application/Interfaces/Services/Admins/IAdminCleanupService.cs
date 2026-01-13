namespace InvenireServer.Application.Interfaces.Services.Admins;

/// <summary>
/// Defines a cleanup service for inactive admins.
/// </summary>
public interface IAdminCleanupService
{
    /// <summary>
    /// Performs cleanup of inactive admins.
    /// </summary>
    /// <returns>Awaitable task representing the cleanup operation.</returns>
    Task CleanupAsync();
}
