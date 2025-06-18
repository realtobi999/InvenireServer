using InvenireServer.Application.Interfaces.Managers;
using InvenireServer.Domain.Entities;
using InvenireServer.Domain.Interfaces.Services.Organizations;

namespace InvenireServer.Application.Services.Organizations;

public class OrganizationInvitationService : IOrganizationInvitationService
{
    private readonly IRepositoryManager _repositories;

    public OrganizationInvitationService(IRepositoryManager repositories)
    {
        _repositories = repositories;
    }

    public async Task CreateAsync(OrganizationInvitation invitation)
    {
        _repositories.OrganizationInvitations.Create(invitation);
        await _repositories.SaveOrThrowAsync();
    }
}