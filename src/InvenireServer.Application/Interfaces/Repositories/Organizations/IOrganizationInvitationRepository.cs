using InvenireServer.Domain.Entities.Organizations;

namespace InvenireServer.Application.Interfaces.Repositories.Organizations;

/// <summary>
/// Defines a repository for organization invitations.
/// </summary>
public interface IOrganizationInvitationRepository : IRepositoryBase<OrganizationInvitation>
{
    /// <summary>
    /// Returns invitations that have expired.
    /// </summary>
    /// <returns>Awaitable task returning expired invitations.</returns>
    Task<IEnumerable<OrganizationInvitation>> IndexExpiredAsync();
}
