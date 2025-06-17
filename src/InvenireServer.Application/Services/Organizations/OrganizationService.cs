using InvenireServer.Application.Interfaces.Common;
using InvenireServer.Application.Interfaces.Managers;
using InvenireServer.Domain.Entities;
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

    public async Task CreateAsync(Organization organization)
    {
        var (valid, exception) = await _validator.ValidateAsync(organization);
        if (!valid && exception is not null) throw exception;

        _repositories.Organizations.Create(organization);
        await _repositories.SaveOrThrowAsync();
    }
}
