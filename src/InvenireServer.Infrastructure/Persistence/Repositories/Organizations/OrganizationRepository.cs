using System.Linq.Expressions;
using InvenireServer.Domain.Entities.Organizations;
using InvenireServer.Domain.Interfaces.Repositories.Organizations;
using Microsoft.EntityFrameworkCore;

namespace InvenireServer.Infrastructure.Persistence.Repositories.Organizations;

public class OrganizationRepository : RepositoryBase<Organization>, IOrganizationRepository
{

    public OrganizationRepository(InvenireServerContext context) : base(context)
    {
        Invitations = new OrganizationInvitationRepository(context);
    }

    public IOrganizationInvitationRepository Invitations { get; }

    public Task<Organization?> GetWithRelationsAsync(Expression<Func<Organization, bool>> predicate)
    {
        return Context.Set<Organization>().Include(o => o.Admin).Include(o => o.Employees).Include(o => o.Invitations).FirstOrDefaultAsync(predicate);
    }
}