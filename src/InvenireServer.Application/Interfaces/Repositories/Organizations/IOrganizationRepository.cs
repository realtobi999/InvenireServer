using InvenireServer.Domain.Entities.Organizations;

namespace InvenireServer.Application.Interfaces.Repositories.Organizations;

public interface IOrganizationRepository : IRepositoryBase<Organization>
{
    IOrganizationInvitationRepository Invitations { get; }
}