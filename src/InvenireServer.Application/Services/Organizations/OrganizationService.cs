using InvenireServer.Application.Interfaces.Managers;
using InvenireServer.Domain.Entities;
using InvenireServer.Domain.Interfaces.Services.Organizations;

namespace InvenireServer.Application.Services.Organizations;

public class OrganizationService : IOrganizationService
{
    private readonly IRepositoryManager _repositories;

    public OrganizationService(IRepositoryManager repositories)
    {
        _repositories = repositories;
    }

    public async Task CreateAsync(Organization organization)
    {
        _repositories.Organizations.Create(organization);
        await _repositories.SaveOrThrowAsync();
    }
}
