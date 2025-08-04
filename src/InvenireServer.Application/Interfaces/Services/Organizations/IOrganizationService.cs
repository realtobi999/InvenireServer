using System.Linq.Expressions;
using InvenireServer.Application.Dtos.Organizations;
using InvenireServer.Application.Interfaces.Services.Organizations.Invitations;
using InvenireServer.Domain.Entities.Organizations;
using InvenireServer.Domain.Entities.Users;

namespace InvenireServer.Application.Interfaces.Services.Organizations;

public interface IOrganizationService
{
    IOrganizationDtoService Dto { get; }
    IOrganizationInvitationService Invitations { get; }
    Task<Organization> GetAsync(Expression<Func<Organization, bool>> predicate);
    Task<Organization?> TryGetForAsync(Admin admin);
    Task<Organization?> TryGetForAsync(Employee employee);
    Task<Organization?> TryGetAsync(Expression<Func<Organization, bool>> predicate);
    Task CreateAsync(Organization organization);
    Task UpdateAsync(Organization organization);
    Task DeleteAsync(Organization organization);
}

public interface IOrganizationDtoService
{
    Task<OrganizationDto> GetAsync(Expression<Func<Organization, bool>> predicate);
    Task<OrganizationDto?> TryGetAsync(Expression<Func<Organization, bool>> predicate);
    Task<OrganizationDto?> TryGetForAsync(Admin admin);
}