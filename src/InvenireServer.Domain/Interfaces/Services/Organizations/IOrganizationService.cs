using System.Linq.Expressions;
using InvenireServer.Domain.Entities.Organizations;

namespace InvenireServer.Domain.Interfaces.Services.Organizations;

public interface IOrganizationService
{
    IOrganizationInvitationService Invitations { get; }

    Task<Organization> GetAsync(Expression<Func<Organization, bool>> predicate);

    Task CreateAsync(Organization organization);
}