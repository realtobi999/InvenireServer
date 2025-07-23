using System.Linq.Expressions;
using FluentValidation;
using FluentValidation.Results;
using InvenireServer.Application.Interfaces.Managers;
using InvenireServer.Domain.Entities.Organizations;
using InvenireServer.Domain.Exceptions.Http;
using InvenireServer.Domain.Interfaces.Services.Organizations.Invitations;
using InvenireServer.Domain.Validators.Organizations;

namespace InvenireServer.Application.Services.Organizations.Invitations;

public class OrganizationInvitationService : IOrganizationInvitationService
{
    private readonly IRepositoryManager _repositories;

    public OrganizationInvitationService(IRepositoryManager repositories)
    {
        _repositories = repositories;
    }

    public async Task<IEnumerable<OrganizationInvitation>> IndexExpiredAsync()
    {
        return await _repositories.Organizations.Invitations.IndexExpiredAsync();
    }

    public async Task<OrganizationInvitation> GetAsync(Expression<Func<OrganizationInvitation, bool>> predicate)
    {
        var invitation = await TryGetAsync(predicate);

        if (invitation is null) throw new NotFound404Exception($"The requested {nameof(OrganizationInvitation).ToLower()} was not found in the system.");

        return invitation;
    }

    public async Task<OrganizationInvitation?> TryGetAsync(Expression<Func<OrganizationInvitation, bool>> predicate)
    {
        var invitation = await _repositories.Organizations.Invitations.GetAsync(predicate);

        return invitation;
    }

    public async Task CreateAsync(OrganizationInvitation invitation)
    {
        var result = new ValidationResult(OrganizationInvitationEntityValidator.Validate(invitation));
        if (!result.IsValid) throw new ValidationException($"One or more core validation errors occurred for {nameof(OrganizationInvitation).ToLower()} (ID: {invitation.Id}).", result.Errors);

        _repositories.Organizations.Invitations.Create(invitation);
        await _repositories.SaveOrThrowAsync();
    }

    public async Task UpdateAsync(OrganizationInvitation invitation)
    {
        var result = new ValidationResult(OrganizationInvitationEntityValidator.Validate(invitation));
        if (!result.IsValid) throw new ValidationException($"One or more core validation errors occurred for {nameof(OrganizationInvitation).ToLower()} (ID: {invitation.Id}).", result.Errors);

        _repositories.Organizations.Invitations.Update(invitation);
        await _repositories.SaveOrThrowAsync();
    }

    public async Task DeleteAsync(OrganizationInvitation invitation)
    {
        await DeleteAsync([invitation]);
    }

    public async Task DeleteAsync(IEnumerable<OrganizationInvitation> invitations)
    {
        foreach (var invitation in invitations) _repositories.Organizations.Invitations.Delete(invitation);

        await _repositories.SaveOrThrowAsync();
    }
}