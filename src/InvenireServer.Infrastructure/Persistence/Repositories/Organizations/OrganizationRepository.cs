using Microsoft.EntityFrameworkCore;
using InvenireServer.Domain.Entities.Users;
using InvenireServer.Domain.Entities.Organizations;
using InvenireServer.Application.Interfaces.Repositories.Organizations;

namespace InvenireServer.Infrastructure.Persistence.Repositories.Organizations;

public class OrganizationRepository : RepositoryBase<Organization>, IOrganizationRepository
{
    public OrganizationRepository(InvenireServerContext context) : base(context)
    {
        Invitations = new OrganizationInvitationRepository(context);
    }

    public IOrganizationInvitationRepository Invitations { get; }

    public async Task<Organization?> GetForAsync(Admin admin)
    {
        return await GetAsync(o => o.Id == admin.OrganizationId);
    }

    public async Task<Organization?> GetForAsync(Employee employee)
    {
        return await GetAsync(o => o.Id == employee.OrganizationId);
    }

    protected override IQueryable<Organization> GetQueryable()
    {
        return base.GetQueryable().Include(o => o.Admin);
    }
}
