using InvenireServer.Domain.Entities.Organizations;

namespace InvenireServer.Domain.Interfaces.Repositories.Organizations;

public interface IOrganizationRepository : IRepositoryBase<Organization>
{
    IOrganizationInvitationRepository Invitations { get; }
}