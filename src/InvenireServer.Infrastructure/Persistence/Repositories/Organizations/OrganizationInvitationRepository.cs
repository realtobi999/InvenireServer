using InvenireServer.Domain.Entities;
using InvenireServer.Domain.Interfaces.Repositories.Organizations;

namespace InvenireServer.Infrastructure.Persistence.Repositories.Organizations;

public class OrganizationInvitationRepository : RepositoryBase<OrganizationInvitation>, IOrganizationInvitationRepository
{
    public OrganizationInvitationRepository(InvenireServerContext context) : base(context)
    {
    }
}