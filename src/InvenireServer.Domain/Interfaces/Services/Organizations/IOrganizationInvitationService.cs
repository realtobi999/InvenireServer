using System.Linq.Expressions;
using InvenireServer.Domain.Entities.Organizations;

namespace InvenireServer.Domain.Interfaces.Services.Organizations;

public interface IOrganizationInvitationService
{
    Task<OrganizationInvitation> GetAsync(Expression<Func<OrganizationInvitation, bool>> predicate);
    Task<OrganizationInvitation> GetWithRelationsAsync(Expression<Func<OrganizationInvitation, bool>> predicate);
    Task CreateAsync(OrganizationInvitation invitation);
    Task DeleteAsync(OrganizationInvitation invitation);
}