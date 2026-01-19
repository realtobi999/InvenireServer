using InvenireServer.Domain.Entities.Properties;
using InvenireServer.Domain.Entities.Users;
using InvenireServer.Domain.Exceptions.Http;

namespace InvenireServer.Domain.Entities.Organizations;

/// <summary>
/// Represents an organization in the domain.
/// </summary>
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

    /// <summary>
    /// Assigns an admin to the organization.
    /// </summary>
    /// <param name="admin">Admin to assign.</param>
    public void AssignAdmin(Admin admin)
    {
        Admin = admin;

        admin.AssignOrganization(this);
    }

    /// <summary>
    /// Assigns a property to the organization.
    /// </summary>
    /// <param name="property">Property to assign.</param>
    public void AssignProperty(Property property)
    {
        Property = property;

        property.AssignOrganization(this);
    }

    /// <summary>
    /// Adds an employee to the organization.
    /// </summary>
    /// <param name="employee">Employee to add.</param>
    public void AddEmployee(Employee employee)
    {
        Employees.Add(employee);
        employee.AssignOrganization(this);
    }

    /// <summary>
    /// Adds employees to the organization.
    /// </summary>
    /// <param name="employees">Employees to add.</param>
    public void AddEmployees(IEnumerable<Employee> employees)
    {
        foreach (var employee in employees) AddEmployee(employee);
    }

    /// <summary>
    /// Removes an employee from the organization.
    /// </summary>
    /// <param name="employee">Employee to remove.</param>
    public void RemoveEmployee(Employee employee)
    {
        employee.UnassignOrganization();
    }

    /// <summary>
    /// Adds an invitation to the organization.
    /// </summary>
    /// <param name="invitation">Invitation to add.</param>
    public void AddInvitation(OrganizationInvitation invitation)
    {
        Invitations.Add(invitation);
        invitation.AssignOrganization(this);
    }

    /// <summary>
    /// Adds invitations to the organization.
    /// </summary>
    /// <param name="invitations">Invitations to add.</param>
    public void AddInvitations(IEnumerable<OrganizationInvitation> invitations)
    {
        foreach (var invitation in invitations) AddInvitation(invitation);
    }

    /// <summary>
    /// Removes an invitation from the organization.
    /// </summary>
    /// <param name="invitation">Invitation to remove.</param>
    public void RemoveInvitation(OrganizationInvitation invitation)
    {
        invitation.UnassignOrganization();
    }
}
