using System.Linq.Expressions;
using InvenireServer.Domain.Entities.Organizations;
using InvenireServer.Application.Dtos.Organizations;

namespace InvenireServer.Application.Interfaces.Repositories.Organizations;

public interface IOrganizationInvitationRepository : IRepositoryBase<OrganizationInvitation>
{
    IOrganizationInvitationDtoRepository Dto { get; }
    Task<IEnumerable<OrganizationInvitation>> IndexExpiredAsync();
}

public interface IOrganizationInvitationDtoRepository
{
    Task<OrganizationInvitationDto?> GetAsync(Expression<Func<OrganizationInvitation, bool>> predicate);
    Task<IEnumerable<OrganizationInvitationDto>> IndexAsync(Expression<Func<OrganizationInvitation, bool>> predicate);
}