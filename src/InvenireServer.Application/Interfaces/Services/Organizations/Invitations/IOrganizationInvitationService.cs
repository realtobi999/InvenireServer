using System.Linq.Expressions;
using InvenireServer.Application.Dtos.Organizations;
using InvenireServer.Domain.Entities.Common;
using InvenireServer.Domain.Entities.Organizations;
using InvenireServer.Domain.Entities.Users;

namespace InvenireServer.Application.Interfaces.Services.Organizations.Invitations;

public interface IOrganizationInvitationService
{
    IOrganizationInvitationDtoService Dto { get; }
    Task<IEnumerable<OrganizationInvitation>> IndexExpiredAsync();
    Task<OrganizationInvitation> GetAsync(Expression<Func<OrganizationInvitation, bool>> predicate);
    Task<OrganizationInvitation?> TryGetAsync(Expression<Func<OrganizationInvitation, bool>> predicate);
    Task CreateAsync(OrganizationInvitation invitation);
    Task UpdateAsync(OrganizationInvitation invitation);
    Task DeleteAsync(OrganizationInvitation invitation);
    Task DeleteAsync(IEnumerable<OrganizationInvitation> invitations);
}

public interface IOrganizationInvitationDtoService
{
    Task<OrganizationInvitationDto> GetAsync(Expression<Func<OrganizationInvitation, bool>> predicate);
    Task<OrganizationInvitationDto?> TryGetAsync(Expression<Func<OrganizationInvitation, bool>> predicate);
    Task<IEnumerable<OrganizationInvitationDto>> IndexAsync(Expression<Func<OrganizationInvitation, bool>> predicate, PaginationParameters pagination);
    Task<IEnumerable<OrganizationInvitationDto>> IndexForAsync(Employee employee, PaginationParameters pagination);
}