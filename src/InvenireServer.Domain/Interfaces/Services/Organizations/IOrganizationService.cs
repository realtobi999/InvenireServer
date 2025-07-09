using System.Linq.Expressions;
using InvenireServer.Domain.Entities.Organizations;
using InvenireServer.Domain.Interfaces.Services.Organizations.Invitations;

namespace InvenireServer.Domain.Interfaces.Services.Organizations;

public interface IOrganizationService
{
    IOrganizationInvitationService Invitations { get; }
    Task<Organization> GetAsync(Expression<Func<Organization, bool>> predicate);
    Task<Organization?> TryGetAsync(Expression<Func<Organization, bool>> predicate);
    Task CreateAsync(Organization organization);
    Task UpdateAsync(Organization organization);
}