using InvenireServer.Domain.Interfaces.Services.Admins;
using InvenireServer.Domain.Interfaces.Services.Employees;

namespace InvenireServer.Application.Interfaces.Managers;

/// <summary>
/// Manages access to service interfaces across the application.
/// </summary>
public interface IServiceManager
{
    IAdminService Admins { get; }

    /// <summary>
    /// Gets the service for employee-related business logic operations.
    /// </summary>
    IEmployeeService Employees { get; }
}