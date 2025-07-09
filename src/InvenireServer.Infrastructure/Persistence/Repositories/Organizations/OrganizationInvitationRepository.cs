using InvenireServer.Domain.Entities.Organizations;
using InvenireServer.Domain.Interfaces.Repositories.Organizations;
using Microsoft.EntityFrameworkCore;

namespace InvenireServer.Infrastructure.Persistence.Repositories.Organizations;

public class OrganizationInvitationRepository : RepositoryBase<OrganizationInvitation>, IOrganizationInvitationRepository
{
    public OrganizationInvitationRepository(InvenireServerContext context) : base(context)
    {
    }

    public Task<IEnumerable<OrganizationInvitation>> IndexExpiredAsync()
    {
        var threshold = DateTimeOffset.UtcNow - OrganizationInvitation.EXPIRATION_TIME;
        return IndexAsync(i => i.CreatedAt <= threshold);
    }

    protected override IQueryable<OrganizationInvitation> GetQueryable()
    {
        return base.GetQueryable().Include(i => i.Employee);
    }
}