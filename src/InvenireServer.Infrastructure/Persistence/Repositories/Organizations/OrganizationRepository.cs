using InvenireServer.Domain.Entities;
using InvenireServer.Domain.Interfaces.Repositories.Organizations;

namespace InvenireServer.Infrastructure.Persistence.Repositories.Organizations;

public class OrganizationRepository : RepositoryBase<Organization>, IOrganizationRepository
{
    public OrganizationRepository(InvenireServerContext context) : base(context)
    {
    }
}