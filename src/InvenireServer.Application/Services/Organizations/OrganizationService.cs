using System.Linq.Expressions;
using InvenireServer.Application.Interfaces.Common;
using InvenireServer.Application.Interfaces.Managers;
using InvenireServer.Domain.Entities.Organizations;
using InvenireServer.Domain.Exceptions.Http;
using InvenireServer.Domain.Interfaces.Services.Organizations;

namespace InvenireServer.Application.Services.Organizations;

public class OrganizationService : IOrganizationService
{
    private readonly IRepositoryManager _repositories;
    private readonly IValidator<Organization> _validator;

    public OrganizationService(IRepositoryManager repositories, IFactoryManager factories)
    {
        _validator = factories.Validators.Initiate<Organization>();
        _repositories = repositories;
    }

    public IOrganizationInvitationService Invitations => new OrganizationInvitationService(_repositories);

    public async Task<Organization> GetAsync(Expression<Func<Organization, bool>> predicate)
    {
        var organization = await _repositories.Organizations.GetAsync(predicate);

        if (organization is null) throw new NotFound404Exception($"The requested {nameof(Organization).ToLower()} was not found in the system.");

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