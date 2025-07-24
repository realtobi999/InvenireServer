using System.Linq.Expressions;
using InvenireServer.Application.Dtos.Organizations;
using InvenireServer.Application.Interfaces.Repositories.Organizations;
using InvenireServer.Domain.Entities.Organizations;
using Microsoft.EntityFrameworkCore;

namespace InvenireServer.Infrastructure.Persistence.Repositories.Organizations;

public class OrganizationRepository : RepositoryBase<Organization>, IOrganizationRepository
{
    public OrganizationRepository(InvenireServerContext context) : base(context)
    {
        Dto = new OrganizationDtoRepository(context);
        Invitations = new OrganizationInvitationRepository(context);
    }

    public IOrganizationDtoRepository Dto { get; }

    public IOrganizationInvitationRepository Invitations { get; }

    protected override IQueryable<Organization> GetQueryable()
    {
        return base.GetQueryable().Include(o => o.Admin);
    }
}

public class OrganizationDtoRepository : IOrganizationDtoRepository
{
    private readonly InvenireServerContext _context;

    public OrganizationDtoRepository(InvenireServerContext context)
    {
        _context = context;
    }

    public async Task<OrganizationDto?> GetAsync(Expression<Func<Organization, bool>> predicate)
    {
        return await _context.Set<Organization>()
            .AsNoTracking()
            .Where(predicate)
            .Select(OrganizationDto.FromOrganizationSelector)
            .FirstOrDefaultAsync();
    }
}