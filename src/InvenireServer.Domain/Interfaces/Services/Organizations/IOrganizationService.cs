using InvenireServer.Domain.Entities;

namespace InvenireServer.Domain.Interfaces.Services.Organizations;

public interface IOrganizationService
{
    Task CreateAsync(Organization organization);
}
