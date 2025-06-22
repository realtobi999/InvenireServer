using InvenireServer.Application.Interfaces.Factories;
using InvenireServer.Application.Interfaces.Factories.Organizations;
using InvenireServer.Application.Interfaces.Factories.Users;

namespace InvenireServer.Application.Interfaces.Managers;

public interface IFactoryManager
{
    IValidatorFactory Validators { get; }

    EntityFactoryGroup Entities { get; }
}

public class EntityFactoryGroup
{
    public required IAdminFactory Admins { get; set; }

    public required IEmployeeFactory Employees { get; set; }

    public required IOrganizationFactory Organization { get; set; }
}