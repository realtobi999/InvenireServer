using InvenireServer.Domain.Entities.Properties;
using InvenireServer.Domain.Entities.Users;
using InvenireServer.Domain.Exceptions.Http;

namespace InvenireServer.Domain.Entities.Organizations;

public class Organization
{
    // Constants.

    public const int MAX_NAME_LENGTH = 155;

    // Core properties.

    public required Guid Id { get; set; }

    public required string Name { get; set; }

    public required DateTimeOffset CreatedAt { get; set; }

    public required DateTimeOffset? LastUpdatedAt { get; set; }

    // Navigational properties.

    public Admin? Admin { get; private set; }

    public Property? Property { get; private set; }

    public ICollection<Employee> Employees { get; private set; } = [];

    public ICollection<OrganizationInvitation> Invitations { get; private set; } = [];

    // Methods.

    public void AssignAdmin(Admin admin)
    {
        if (Admin is not null) throw new BadRequest400Exception("A admin is already assigned to this organization.");

        Admin = admin;

        admin.AssignOrganization(this);
    }

    public void AssignProperty(Property property)
    {
        if (Property is not null) throw new BadRequest400Exception("A property is already assigned to this organization");

        Property = property;

        property.AssignOrganization(this);
    }

    public void AddEmployee(Employee employee)
    {
        if (Employees.Any(e => e.Id == employee.Id)) throw new BadRequest400Exception("This employee is already a part of this organization.");

        Employees.Add(employee);

        employee.AssignOrganization(this);
    }

    public void AddInvitation(OrganizationInvitation invitation)
    {
        if (Invitations.Any(i => i.Id == invitation.Id)) throw new BadRequest400Exception("This invitation is already a part of this organization.");

        Invitations.Add(invitation);

        invitation.AssignOrganization(this);
    }
}