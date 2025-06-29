using System.Linq.Expressions;
using InvenireServer.Domain.Entities.Organizations;
using InvenireServer.Domain.Interfaces.Repositories.Organizations;
using Microsoft.EntityFrameworkCore;

namespace InvenireServer.Infrastructure.Persistence.Repositories.Organizations;

public class OrganizationInvitationRepository : RepositoryBase<OrganizationInvitation>, IOrganizationInvitationRepository
{
    public OrganizationInvitationRepository(InvenireServerContext context) : base(context)
    {
    }

    public Task<OrganizationInvitation?> GetWithRelationsAsync(Expression<Func<OrganizationInvitation, bool>> predicate)
    {
        return Context.Set<OrganizationInvitation>().Include(i => i.Employee).FirstOrDefaultAsync(predicate);
    }
}