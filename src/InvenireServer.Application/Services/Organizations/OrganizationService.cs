using System.Linq.Expressions;
using FluentValidation;
using FluentValidation.Results;
using InvenireServer.Application.Interfaces.Managers;
using InvenireServer.Application.Services.Organizations.Invitations;
using InvenireServer.Domain.Entities.Organizations;
using InvenireServer.Domain.Exceptions.Http;
using InvenireServer.Domain.Interfaces.Services.Organizations;
using InvenireServer.Domain.Interfaces.Services.Organizations.Invitations;
using InvenireServer.Domain.Validators.Organizations;

namespace InvenireServer.Application.Services.Organizations;

public class OrganizationService : IOrganizationService
{
    private readonly IRepositoryManager _repositories;

    public OrganizationService(IRepositoryManager repositories)
    {
        _repositories = repositories;

        Invitations = new OrganizationInvitationService(repositories);
    }

    public IOrganizationInvitationService Invitations { get; }

    public async Task<Organization> GetAsync(Expression<Func<Organization, bool>> predicate)
    {
        var organization = await TryGetAsync(predicate);

        if (organization is null) throw new NotFound404Exception($"The requested {nameof(Organization).ToLower()} was not found in the system.");

        return organization;
    }

    public async Task<Organization?> TryGetAsync(Expression<Func<Organization, bool>> predicate)
    {
        var organization = await _repositories.Organizations.GetAsync(predicate);

        return organization;
    }

    public async Task CreateAsync(Organization organization)
    {
        var result = new ValidationResult(OrganizationEntityValidator.Validate(organization));
        if (!result.IsValid) throw new ValidationException($"One or more core validation errors occurred for {nameof(Organization).ToLower()} (ID: {organization.Id}).", result.Errors);

        _repositories.Organizations.Create(organization);
        await _repositories.SaveOrThrowAsync();
    }

    public async Task UpdateAsync(Organization organization)
    {
        organization.LastUpdatedAt = DateTimeOffset.UtcNow;

        var result = new ValidationResult(OrganizationEntityValidator.Validate(organization));
        if (!result.IsValid) throw new ValidationException($"One or more core validation errors occurred for {nameof(Organization).ToLower()} (ID: {organization.Id}).", result.Errors);

        _repositories.Organizations.Update(organization);
        await _repositories.SaveOrThrowAsync();
    }
}