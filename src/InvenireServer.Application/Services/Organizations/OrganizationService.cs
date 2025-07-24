using System.Linq.Expressions;
using FluentValidation;
using FluentValidation.Results;
using InvenireServer.Application.Dtos.Organizations;
using InvenireServer.Application.Interfaces.Managers;
using InvenireServer.Application.Interfaces.Services.Organizations;
using InvenireServer.Application.Interfaces.Services.Organizations.Invitations;
using InvenireServer.Application.Services.Organizations.Invitations;
using InvenireServer.Domain.Entities.Organizations;
using InvenireServer.Domain.Entities.Users;
using InvenireServer.Domain.Exceptions.Http;
using InvenireServer.Domain.Validators.Organizations;

namespace InvenireServer.Application.Services.Organizations;

public class OrganizationService : IOrganizationService
{
    private readonly IRepositoryManager _repositories;

    public OrganizationService(IRepositoryManager repositories)
    {
        _repositories = repositories;

        Dto = new OrganizationDtoService(repositories);
        Invitations = new OrganizationInvitationService(repositories);
    }

    public IOrganizationDtoService Dto { get; }

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

    public async Task<Organization?> TryGetForAsync(Admin admin)
    {
        var organization = await TryGetAsync(o => o.Id == admin.OrganizationId);

        return organization;
    }

    public async Task<Organization?> TryGetForAsync(Employee employee)
    {
        var organization = await TryGetAsync(o => o.Id == employee.OrganizationId);

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

    public async Task DeleteAsync(Organization organization)
    {
        _repositories.Organizations.Delete(organization);
        await _repositories.SaveOrThrowAsync();
    }
}

public class OrganizationDtoService : IOrganizationDtoService
{
    private readonly IRepositoryManager _repositories;

    public OrganizationDtoService(IRepositoryManager repositories)
    {
        _repositories = repositories;
    }

    public async Task<OrganizationDto> GetAsync(Expression<Func<Organization, bool>> predicate)
    {
        var organizationDto = await TryGetAsync(predicate);

        if (organizationDto is null) throw new NotFound404Exception($"The requested {nameof(Organization).ToLower()} was not found in the system.");

        return organizationDto;
    }

    public async Task<OrganizationDto?> TryGetAsync(Expression<Func<Organization, bool>> predicate)
    {
        var organizationDto = await _repositories.Organizations.Dto.GetAsync(predicate);

        return organizationDto;
    }

    public async Task<OrganizationDto?> TryGetForAsync(Admin admin)
    {
        var organizationDto = await TryGetAsync(o => o.Id == admin.OrganizationId);

        return organizationDto;
    }
}