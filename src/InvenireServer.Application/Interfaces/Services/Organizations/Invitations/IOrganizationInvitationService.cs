using System.Linq.Expressions;
using InvenireServer.Domain.Entities.Organizations;

namespace InvenireServer.Application.Interfaces.Services.Organizations.Invitations;

public interface IOrganizationInvitationService
{
    Task<IEnumerable<OrganizationInvitation>> IndexExpiredAsync();
    Task<OrganizationInvitation> GetAsync(Expression<Func<OrganizationInvitation, bool>> predicate);
    Task<OrganizationInvitation?> TryGetAsync(Expression<Func<OrganizationInvitation, bool>> predicate);
    Task CreateAsync(OrganizationInvitation invitation);
    Task UpdateAsync(OrganizationInvitation invitation);
    Task DeleteAsync(OrganizationInvitation invitation);
    Task DeleteAsync(IEnumerable<OrganizationInvitation> invitations);
}