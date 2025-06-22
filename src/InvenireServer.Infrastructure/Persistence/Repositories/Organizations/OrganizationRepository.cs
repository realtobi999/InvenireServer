using InvenireServer.Domain.Entities.Organizations;
using InvenireServer.Domain.Interfaces.Repositories.Organizations;
using Microsoft.EntityFrameworkCore;

namespace InvenireServer.Infrastructure.Persistence.Repositories.Organizations;

public class OrganizationRepository : RepositoryBase<Organization>, IOrganizationRepository
{
    private readonly InvenireServerContext _context;

    public OrganizationRepository(InvenireServerContext context) : base(context)
    {
        _context = context;
    }

    public IOrganizationInvitationRepository Invitations => new OrganizationInvitationRepository(_context);

    protected override IQueryable<Organization> GetQueryable()
    {
        return base.GetQueryable()
            .Include(o => o.Admin)
            .Include(o => o.Employees)
            .Include(o => o.Invitations)
            .ThenInclude(i => i.Employee);
    }
}