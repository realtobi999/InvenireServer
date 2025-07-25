using System.Linq.Expressions;
using InvenireServer.Application.Dtos.Organizations;
using InvenireServer.Application.Interfaces.Repositories.Organizations;
using InvenireServer.Domain.Entities.Organizations;
using InvenireServer.Domain.Entities.Users;
using Microsoft.EntityFrameworkCore;

namespace InvenireServer.Infrastructure.Persistence.Repositories.Organizations;

public class OrganizationInvitationRepository : RepositoryBase<OrganizationInvitation>, IOrganizationInvitationRepository
{
    public OrganizationInvitationRepository(InvenireServerContext context) : base(context)
    {
        Dto = new OrganizationInvitationDtoRepository(context);
    }

    public IOrganizationInvitationDtoRepository Dto { get; }

    public Task<IEnumerable<OrganizationInvitation>> IndexExpiredAsync()
    {
        var threshold = DateTimeOffset.UtcNow.Add(-OrganizationInvitation.EXPIRATION_TIME);
        return IndexAsync(i => i.CreatedAt <= threshold);
    }

    protected override IQueryable<OrganizationInvitation> GetQueryable()
    {
        return base.GetQueryable().Include(i => i.Employee);
    }
}

public class OrganizationInvitationDtoRepository : IOrganizationInvitationDtoRepository
{
    private readonly InvenireServerContext _context;

    public OrganizationInvitationDtoRepository(InvenireServerContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<OrganizationInvitationDto>> IndexAsync(Expression<Func<OrganizationInvitation, bool>> predicate)
    {
        return await _context.Set<OrganizationInvitation>()
            .AsNoTracking()
            .Where(predicate)
            .Select(OrganizationInvitationDto.FromInvitationSelector)
            .ToListAsync();
    }
}
