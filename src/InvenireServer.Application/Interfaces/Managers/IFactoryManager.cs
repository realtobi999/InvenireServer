using System.Numerics;
using InvenireServer.Application.Interfaces.Factories;
using InvenireServer.Application.Interfaces.Factories.Admins;
using InvenireServer.Application.Interfaces.Factories.Employees;

namespace InvenireServer.Application.Interfaces.Managers;

/// <summary>
/// Manages access to various factory services used throughout the application.
/// </summary>
public interface IFactoryManager
{
    /// <summary>
    /// Gets the factory responsible for creating validator instances.
    /// </summary>
    IValidatorFactory Validators { get; }

    IEntityFactoryGroup Entities { get; }
}

public interface IEntityFactoryGroup
{
    IAdminFactory Admins { get; }
    IEmployeeFactory Employees { get; }
}

public class EntityFactoryGroup : IEntityFactoryGroup
{
    public required IAdminFactory Admins { get; set; }

    public required IEmployeeFactory Employees { get; set; }
}