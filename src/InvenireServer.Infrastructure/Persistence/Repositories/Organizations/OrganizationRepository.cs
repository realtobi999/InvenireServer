using InvenireServer.Application.Interfaces.Repositories.Organizations;
using InvenireServer.Domain.Entities.Organizations;
using Microsoft.EntityFrameworkCore;

namespace InvenireServer.Infrastructure.Persistence.Repositories.Organizations;

public class OrganizationRepository : RepositoryBase<Organization>, IOrganizationRepository
{
    public OrganizationRepository(InvenireServerContext context) : base(context)
    {
        Invitations = new OrganizationInvitationRepository(context);
    }

    public IOrganizationInvitationRepository Invitations { get; }

    protected override IQueryable<Organization> GetQueryable()
    {
        return base.GetQueryable().Include(o => o.Admin);
    }
}