using InvenireServer.Domain.Entities.Organizations;
using InvenireServer.Domain.Interfaces.Repositories.Organizations;

namespace InvenireServer.Infrastructure.Persistence.Repositories.Organizations;

public class OrganizationRepository : RepositoryBase<Organization>, IOrganizationRepository
{

    public OrganizationRepository(InvenireServerContext context) : base(context)
    {
        Invitations = new OrganizationInvitationRepository(context);
    }

    public IOrganizationInvitationRepository Invitations { get; }
}