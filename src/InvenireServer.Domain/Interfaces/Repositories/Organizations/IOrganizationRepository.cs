using System.Linq.Expressions;
using InvenireServer.Domain.Entities.Organizations;

namespace InvenireServer.Domain.Interfaces.Repositories.Organizations;

public interface IOrganizationRepository : IRepositoryBase<Organization>
{
    IOrganizationInvitationRepository Invitations { get; }

    Task<Organization?> GetWithRelationsAsync(Expression<Func<Organization, bool>> predicate);
}