using InvenireServer.Application.Factories.Organizations;
using InvenireServer.Application.Factories.Users;
using InvenireServer.Application.Interfaces.Factories;
using InvenireServer.Application.Interfaces.Managers;

namespace InvenireServer.Application.Factories;

public class FactoryManager : IFactoryManager
{
    private readonly Lazy<EntityFactoryGroup> _entities;
    private readonly Lazy<IValidatorFactory> _validators;

    public FactoryManager(IValidatorFactory validators)
    {
        _entities = new Lazy<EntityFactoryGroup>(new EntityFactoryGroup
        {
            Admins = new AdminFactory(),
            Employees = new EmployeeFactory(),
            Organization = new OrganizationFactory(),
        });
        _validators = new Lazy<IValidatorFactory>(validators);
    }

    public IValidatorFactory Validators => _validators.Value;

    public EntityFactoryGroup Entities => _entities.Value;
}