using System.Linq.Expressions;
using InvenireServer.Application.Dtos.Organizations;
using InvenireServer.Domain.Entities.Organizations;
using InvenireServer.Domain.Entities.Users;

namespace InvenireServer.Application.Interfaces.Repositories.Organizations;

public interface IOrganizationInvitationRepository : IRepositoryBase<OrganizationInvitation>
{
    IOrganizationInvitationDtoRepository Dto { get; }
    Task<IEnumerable<OrganizationInvitation>> IndexExpiredAsync();
}

public interface IOrganizationInvitationDtoRepository
{
    Task<IEnumerable<OrganizationInvitationDto>> IndexAsync(Expression<Func<OrganizationInvitation, bool>> predicate);
}