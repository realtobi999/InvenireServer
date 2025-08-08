using InvenireServer.Domain.Entities.Properties;
using InvenireServer.Domain.Entities.Users;
using InvenireServer.Domain.Exceptions.Http;

namespace InvenireServer.Domain.Entities.Organizations;

public class Organization
{
    // Constants.

    public const int MAX_NAME_LENGTH = 155;

    public const int MAX_INVITATIONS = 100;

    // Core properties.

    public required Guid Id { get; set; }

    public required string Name { get; set; }

    public required DateTimeOffset CreatedAt { get; set; }

    public required DateTimeOffset? LastUpdatedAt { get; set; }

    // Navigational properties.

    public Admin? Admin { get; set; }

    public Property? Property { get; private set; }

    public ICollection<Employee> Employees { get; } = [];

    public ICollection<OrganizationInvitation> Invitations { get; } = [];

    // Methods.

    public void AssignAdmin(Admin admin)
    {
        Admin = admin;

        admin.AssignOrganization(this);
    }

    public void AssignProperty(Property property)
    {
        Property = property;

        property.AssignOrganization(this);
    }

    public void AddEmployee(Employee employee)
    {
        Employees.Add(employee);
        employee.AssignOrganization(this);
    }

    public void AddEmployees(IEnumerable<Employee> employees)
    {
        foreach (var employee in employees) AddEmployee(employee);
    }

    public void RemoveEmployee(Employee employee)
    {
        employee.UnassignOrganization();
    }

    public void AddInvitation(OrganizationInvitation invitation)
    {
        Invitations.Add(invitation);
        invitation.AssignOrganization(this);
    }

    public void AddInvitations(IEnumerable<OrganizationInvitation> invitations)
    {
        foreach (var invitation in invitations) AddInvitation(invitation);
    }

    public void RemoveInvitation(OrganizationInvitation invitation)
    {
        invitation.UnassignOrganization();
    }
}