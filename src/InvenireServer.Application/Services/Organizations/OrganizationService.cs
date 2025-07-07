using System.Linq.Expressions;
using InvenireServer.Application.Interfaces.Common;
using InvenireServer.Application.Interfaces.Factories;
using InvenireServer.Application.Interfaces.Managers;
using InvenireServer.Domain.Entities.Organizations;
using InvenireServer.Domain.Exceptions.Http;
using InvenireServer.Domain.Interfaces.Services.Organizations;

namespace InvenireServer.Application.Services.Organizations;

public class OrganizationService : IOrganizationService
{
    private readonly IRepositoryManager _repositories;
    private readonly IEntityValidator<Organization> _validator;

    public OrganizationService(IRepositoryManager repositories, IEntityValidatorFactory validators)
    {
        _validator = validators.Initiate<Organization>();
        _repositories = repositories;

        Invitations = new OrganizationInvitationService(repositories, validators.Initiate<OrganizationInvitation>());
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
        var (valid, exception) = await _validator.ValidateAsync(organization);
        if (!valid && exception is not null) throw exception;

        _repositories.Organizations.Create(organization);
        await _repositories.SaveOrThrowAsync();
    }

    public async Task UpdateAsync(Organization organization)
    {
        organization.LastUpdatedAt = DateTimeOffset.UtcNow;

        var (valid, exception) = await _validator.ValidateAsync(organization);
        if (!valid && exception is not null) throw exception;

        _repositories.Organizations.Update(organization);
        await _repositories.SaveOrThrowAsync();
    }
}