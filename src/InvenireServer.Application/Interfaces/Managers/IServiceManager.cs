using InvenireServer.Domain.Interfaces.Services;

namespace InvenireServer.Application.Interfaces.Managers;

/// <summary>
/// Manages access to service interfaces across the application.
/// </summary>
public interface IServiceManager
{
    /// <summary>
    /// Gets the service for employee-related business logic operations.
    /// </summary>
    IEmployeeService Employees { get; }
}