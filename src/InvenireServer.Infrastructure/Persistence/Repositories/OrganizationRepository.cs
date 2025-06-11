using InvenireServer.Domain.Entities;
using InvenireServer.Domain.Interfaces.Repositories;

namespace InvenireServer.Infrastructure.Persistence.Repositories;

public class OrganizationRepository : BaseRepository<Organization>, IOrganizationRepository
{
    public OrganizationRepository(InvenireServerContext context) : base(context)
    {
    }
}
