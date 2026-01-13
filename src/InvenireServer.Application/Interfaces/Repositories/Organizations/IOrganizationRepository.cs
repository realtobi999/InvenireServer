using InvenireServer.Domain.Entities.Organizations;
using InvenireServer.Domain.Entities.Users;

namespace InvenireServer.Application.Interfaces.Repositories.Organizations;

/// <summary>
/// Defines a repository for organizations.
/// </summary>
public interface IOrganizationRepository : IRepositoryBase<Organization>
{
    /// <summary>
    /// Repository for organization invitations.
    /// </summary>
    IOrganizationInvitationRepository Invitations { get; }

    /// <summary>
    /// Gets the organization for the specified admin.
    /// </summary>
    /// <param name="admin">Admin to resolve the organization for.</param>
    /// <returns>Awaitable task returning the organization or null.</returns>
    Task<Organization?> GetForAsync(Admin admin);

    /// <summary>
    /// Gets the organization for the specified employee.
    /// </summary>
    /// <param name="employee">Employee to resolve the organization for.</param>
    /// <returns>Awaitable task returning the organization or null.</returns>
    Task<Organization?> GetForAsync(Employee employee);
}
