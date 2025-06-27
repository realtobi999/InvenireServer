using System.Linq.Expressions;
using InvenireServer.Application.Interfaces.Common;
using InvenireServer.Application.Interfaces.Managers;
using InvenireServer.Domain.Entities.Organizations;
using InvenireServer.Domain.Exceptions.Http;
using InvenireServer.Domain.Interfaces.Services.Organizations;

namespace InvenireServer.Application.Services.Organizations;

public class OrganizationInvitationService : IOrganizationInvitationService
{
    private readonly IRepositoryManager _repositories;
    private readonly IValidator<OrganizationInvitation> _validator;

    public OrganizationInvitationService(IRepositoryManager repositories, IValidator<OrganizationInvitation> validator)
    {
        _repositories = repositories;
        _validator = validator;
    }

    public async Task<OrganizationInvitation> GetAsync(Expression<Func<OrganizationInvitation, bool>> predicate)
    {
        var invitation = await _repositories.Organizations.Invitations.GetAsync(predicate);

        if (invitation is null) throw new NotFound404Exception("The requested invitation was not found in the system.");

        return invitation;
    }

    public async Task CreateAsync(OrganizationInvitation invitation)
    {
        var (valid, exception) = await _validator.ValidateAsync(invitation);
        if (!valid && exception is not null) throw exception;

        _repositories.Organizations.Invitations.Create(invitation);
        await _repositories.SaveOrThrowAsync();
    }

    public async Task DeleteAsync(OrganizationInvitation invitation)
    {
        var (valid, exception) = await _validator.ValidateAsync(invitation);
        if (!valid && exception is not null) throw exception;

        _repositories.Organizations.Invitations.Delete(invitation);
        await _repositories.SaveOrThrowAsync();
    }
}