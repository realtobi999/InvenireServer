using Microsoft.EntityFrameworkCore;
using InvenireServer.Domain.Entities.Organizations;
using InvenireServer.Application.Interfaces.Repositories.Organizations;

namespace InvenireServer.Infrastructure.Persistence.Repositories.Organizations;

/// <summary>
/// Default implementation of <see cref="IOrganizationInvitationRepository"/>.
/// </summary>
public class OrganizationInvitationRepository : RepositoryBase<OrganizationInvitation>, IOrganizationInvitationRepository
{
    public OrganizationInvitationRepository(InvenireServerContext context) : base(context)
    {
    }

    /// <summary>
    /// Returns invitations that have expired.
    /// </summary>
    /// <returns>Awaitable task returning expired invitations.</returns>
    public Task<IEnumerable<OrganizationInvitation>> IndexExpiredAsync()
    {
        var threshold = DateTimeOffset.UtcNow.Add(-OrganizationInvitation.EXPIRATION_TIME);
        return IndexAsync(i => i.CreatedAt <= threshold);
    }

    /// <summary>
    /// Updates an organization invitation entity in the repository.
    /// </summary>
    /// <param name="invitation">Organization invitation to update.</param>
    public override void Update(OrganizationInvitation invitation)
    {
        invitation.LastUpdatedAt = DateTimeOffset.UtcNow;
        base.Update(invitation);
    }

    /// <summary>
    /// Returns the base queryable including related employee data.
    /// </summary>
    /// <returns>Queryable for organization invitations.</returns>
    protected override IQueryable<OrganizationInvitation> GetQueryable()
    {
        return base.GetQueryable().Include(i => i.Employee);
    }
}
