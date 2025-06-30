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

    public Admin? Admin { get; set; }

    public Property? Property { get; set; }

    public ICollection<Employee> Employees { get; set; } = [];

    public ICollection<OrganizationInvitation> Invitations { get; set; } = [];

    // Methods.

    public void AssignAdmin(Admin admin)
    {
        if (this.Admin is not null) throw new BadRequest400Exception("A admin is already assigned to this organization.");

        this.Admin = admin;

        admin.AssignOrganization(this);
    }

    public void AssignProperty(Property property)
    {
        if (this.Property is not null) throw new BadRequest400Exception("A property is already assigned to this organization");

        this.Property = property;

        property.AssignOrganization(this);
    }

    public void AddEmployee(Employee employee)
    {
        if (this.Employees.Any(e => e.Id == employee.Id)) throw new BadRequest400Exception("This employee is already a part of this organization.");

        this.Employees.Add(employee);

        employee.AssignOrganization(this);
    }

    public void AddInvitation(OrganizationInvitation invitation)
    {
        if (this.Invitations.Any(i => i.Id == invitation.Id)) throw new BadRequest400Exception("This invitation is already a part of this organization.");

        this.Invitations.Add(invitation);

        invitation.AssignOrganization(this);
    }
}