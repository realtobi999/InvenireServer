using InvenireServer.Application.Factories.Admins;
using InvenireServer.Application.Factories.Employees;
using InvenireServer.Application.Interfaces.Factories;
using InvenireServer.Application.Interfaces.Managers;

namespace InvenireServer.Application.Factories;

/// <summary>
/// Provides centralized access to factory services.
/// </summary>
public class FactoryManager : IFactoryManager
{
    private readonly Lazy<IValidatorFactory> _validators;
    private readonly Lazy<IEntityFactoryGroup> _entities;

    /// <summary>
    /// Initializes a new instance of the <see cref="FactoryManager"/> class.
    /// </summary>
    /// <param name="validators">The factory responsible for creating validators.</param>
    public FactoryManager(IValidatorFactory validators)
    {
        _entities = new Lazy<IEntityFactoryGroup>(new EntityFactoryGroup
        {
            Admins = new AdminFactory(),
            Employees = new EmployeeFactory(),
        });
        _validators = new Lazy<IValidatorFactory>(validators);
    }

    /// <inheritdoc/>
    public IValidatorFactory Validators => _validators.Value;

    /// <inheritdoc/>
    public IEntityFactoryGroup Entities => _entities.Value;
}