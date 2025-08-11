using InvenireServer.Domain.Entities.Organizations;
using InvenireServer.Domain.Entities.Users;

namespace InvenireServer.Application.Interfaces.Repositories.Organizations;

public interface IOrganizationRepository : IRepositoryBase<Organization>
{
    IOrganizationInvitationRepository Invitations { get; }
    Task<Organization?> GetForAsync(Admin admin);
    Task<Organization?> GetForAsync(Employee employee);
}