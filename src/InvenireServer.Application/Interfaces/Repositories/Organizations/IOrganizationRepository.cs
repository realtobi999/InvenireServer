using System.Linq.Expressions;
using InvenireServer.Application.Dtos.Organizations;
using InvenireServer.Domain.Entities.Organizations;

namespace InvenireServer.Application.Interfaces.Repositories.Organizations;

public interface IOrganizationRepository : IRepositoryBase<Organization>
{
    IOrganizationDtoRepository Dto { get; }
    IOrganizationInvitationRepository Invitations { get; }
}

public interface IOrganizationDtoRepository
{
    Task<OrganizationDto?> GetAsync(Expression<Func<Organization, bool>> predicate);
}