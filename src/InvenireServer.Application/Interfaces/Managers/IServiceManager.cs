using InvenireServer.Domain.Interfaces.Services.Admins;
using InvenireServer.Domain.Interfaces.Services.Employees;
using InvenireServer.Domain.Interfaces.Services.Organizations;

namespace InvenireServer.Application.Interfaces.Managers;

public interface IServiceManager
{
    IAdminService Admins { get; }

    IEmployeeService Employees { get; }

    IOrganizationService Organizations { get; }

    IOrganizationInvitationService OrganizationInvitations { get; }
}