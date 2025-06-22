using System.Linq.Expressions;
using InvenireServer.Domain.Exceptions.Http;
using InvenireServer.Domain.Entities.Organizations;
using InvenireServer.Application.Interfaces.Managers;
using InvenireServer.Domain.Interfaces.Services.Organizations;

namespace InvenireServer.Application.Services.Organizations;

public class OrganizationInvitationService : IOrganizationInvitationService
{
    private readonly IRepositoryManager _repositories;

    public OrganizationInvitationService(IRepositoryManager repositories)
    {
        _repositories = repositories;
    }

    public async Task<OrganizationInvitation> GetAsync(Expression<Func<OrganizationInvitation, bool>> predicate)
    {
        var invitation = await _repositories.Organizations.Invitations.GetAsync(predicate);

        if (invitation is null) throw new NotFound404Exception("The requested invitation was not found in the system.");

        return invitation;
    }

    public async Task CreateAsync(OrganizationInvitation invitation)
    {
        _repositories.Organizations.Invitations.Create(invitation);
        await _repositories.SaveOrThrowAsync();
    }

    public async Task DeleteAsync(OrganizationInvitation invitation)
    {
        _repositories.Organizations.Invitations.Delete(invitation);
        await _repositories.SaveOrThrowAsync();
    }
}