namespace InvenireServer.Application.Interfaces.Services.Organizations.Invitations;

/// <summary>
/// Defines a cleanup service for expired organization invitations.
/// </summary>
public interface IOrganizationInvitationCleanupService
{
    /// <summary>
    /// Performs cleanup of expired organization invitations.
    /// </summary>
    /// <returns>Awaitable task representing the cleanup operation.</returns>
    Task CleanupAsync();
}
