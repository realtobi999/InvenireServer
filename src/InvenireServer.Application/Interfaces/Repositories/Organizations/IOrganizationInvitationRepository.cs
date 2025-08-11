using InvenireServer.Domain.Entities.Organizations;

namespace InvenireServer.Application.Interfaces.Repositories.Organizations;

public interface IOrganizationInvitationRepository : IRepositoryBase<OrganizationInvitation>
{
    Task<IEnumerable<OrganizationInvitation>> IndexExpiredAsync();
}