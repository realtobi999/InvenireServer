using System.Linq.Expressions;
using InvenireServer.Application.Interfaces.Services.Organizations.Invitations;
using InvenireServer.Domain.Entities.Organizations;
using InvenireServer.Domain.Entities.Users;

namespace InvenireServer.Application.Interfaces.Services.Organizations;

public interface IOrganizationService
{
    IOrganizationInvitationService Invitations { get; }
    Task<Organization> GetAsync(Expression<Func<Organization, bool>> predicate);
    Task<Organization?> TryGetForAsync(Admin admin);
    Task<Organization?> TryGetForAsync(Employee employee);
    Task<Organization?> TryGetAsync(Expression<Func<Organization, bool>> predicate);
    Task CreateAsync(Organization organization);
    Task UpdateAsync(Organization organization);
    Task DeleteAsync(Organization organization);
}