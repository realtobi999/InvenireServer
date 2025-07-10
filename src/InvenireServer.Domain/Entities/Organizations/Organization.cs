using InvenireServer.Domain.Entities.Properties;
using InvenireServer.Domain.Entities.Users;
using InvenireServer.Domain.Exceptions.Http;

namespace InvenireServer.Domain.Entities.Organizations;

public class Organization
{
    // Constants.

    public const int MAX_NAME_LENGTH = 155;

    public const int MAX_AMOUNT_OF_INVITATIONS = 100;

    // Core properties.

    public required Guid Id { get; set; }

    public required string Name { get; set; }

    public required DateTimeOffset CreatedAt { get; set; }

    public required DateTimeOffset? LastUpdatedAt { get; set; }

    // Navigational properties.

    public Admin? Admin { get; private set; }

    public Property? Property { get; private set; }

    public ICollection<Employee> Employees { get; } = [];

    public ICollection<OrganizationInvitation> Invitations { get; } = [];

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

    public void AddEmployees(IEnumerable<Employee> employees)
    {
        foreach (var employee in employees) AddEmployee(employee);
    }

    public void AddInvitation(OrganizationInvitation invitation)
    {
        if (Invitations.Any(i => i.Id == invitation.Id)) throw new BadRequest400Exception("This invitation is already a part of this organization.");
        if (Invitations.Count > MAX_AMOUNT_OF_INVITATIONS) throw new BadRequest400Exception("Maximum number of invitations reached.");

        Invitations.Add(invitation);

        invitation.AssignOrganization(this);
    }

    public void AddInvitations(IEnumerable<OrganizationInvitation> invitations)
    {
        foreach (var invitation in invitations) AddInvitation(invitation);
    }

    public void RemoveInvitation(OrganizationInvitation invitation)
    {
        if (Invitations.All(i => i.Id != invitation.Id)) throw new BadRequest400Exception("This invitation is not a part of this organization.");

        Invitations.Remove(invitation);

        invitation.UnassignOrganization();
    }
}