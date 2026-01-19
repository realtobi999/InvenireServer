using Microsoft.EntityFrameworkCore;
using InvenireServer.Domain.Entities.Users;
using InvenireServer.Domain.Entities.Organizations;
using InvenireServer.Application.Interfaces.Repositories.Organizations;

namespace InvenireServer.Infrastructure.Persistence.Repositories.Organizations;

/// <summary>
/// Default implementation of <see cref="IOrganizationRepository"/>.
/// </summary>
public class OrganizationRepository : RepositoryBase<Organization>, IOrganizationRepository
{
    public OrganizationRepository(InvenireServerContext context) : base(context)
    {
        Invitations = new OrganizationInvitationRepository(context);
    }

    public IOrganizationInvitationRepository Invitations { get; }

    /// <summary>
    /// Gets the organization for the specified admin.
    /// </summary>
    /// <param name="admin">Admin to resolve the organization for.</param>
    /// <returns>Awaitable task returning the organization or null.</returns>
    public async Task<Organization?> GetForAsync(Admin admin)
    {
        return await GetAsync(o => o.Id == admin.OrganizationId);
    }

    /// <summary>
    /// Gets the organization for the specified employee.
    /// </summary>
    /// <param name="employee">Employee to resolve the organization for.</param>
    /// <returns>Awaitable task returning the organization or null.</returns>
    public async Task<Organization?> GetForAsync(Employee employee)
    {
        return await GetAsync(o => o.Id == employee.OrganizationId);
    }

    /// <summary>
    /// Updates an organization entity in the repository.
    /// </summary>
    /// <param name="organization">Organization to update.</param>
    public override void Update(Organization organization)
    {
        organization.LastUpdatedAt = DateTimeOffset.UtcNow;
        base.Update(organization);
    }

    /// <summary>
    /// Returns the base queryable including related admin data.
    /// </summary>
    /// <returns>Queryable for organizations.</returns>
    protected override IQueryable<Organization> GetQueryable()
    {
        return base.GetQueryable().Include(o => o.Admin);
    }
}
