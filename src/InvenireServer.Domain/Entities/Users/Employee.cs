using System.Collections.ObjectModel;
using InvenireServer.Domain.Entities.Organizations;
using InvenireServer.Domain.Entities.Properties;
using InvenireServer.Domain.Exceptions.Http;

namespace InvenireServer.Domain.Entities.Users;

public class Employee
{
    // Constants.

    public const int MAX_NAME_LENGTH = 155;

    public const int MAX_EMAIL_ADDRESS_LENGTH = 155;

    public const int MIN_PASSWORD_LENGTH = 8;

    public const int MAX_PASSWORD_LENGTH = 155;

    public static readonly TimeSpan INACTIVE_THRESHOLD = TimeSpan.FromDays(14);

    // Core Properties.

    public required Guid Id { get; init; }

    public required string Name { get; set; }

    public required string Password { get; set; }

    public required string EmailAddress { get; set; }

    public required bool IsVerified { get; set; }

    public required DateTimeOffset CreatedAt { get; set; }

    public required DateTimeOffset? LastUpdatedAt { get; set; }

    public required DateTimeOffset? LastLoginAt { get; set; }

    // Navigational Properties.

    public Guid? OrganizationId { get; set; }

    public Collection<PropertyItem> AssignedItems { get; set; } = [];

    // Methods.

    public void Verify()
    {
        if (IsVerified) throw new BadRequest400Exception("Employee is already verified.");

        IsVerified = true;
    }

    public void AssignOrganization(Organization organization)
    {
        if (OrganizationId is not null) throw new BadRequest400Exception("This employee is already part of a another organization");
        OrganizationId = organization.Id;
    }

    public void UnassignOrganization(Organization organization)
    {
        if (OrganizationId is null) throw new BadRequest400Exception("This employee is not part of a any organization");
        if (OrganizationId != organization.Id) throw new BadRequest400Exception("Cannot unassign a organization that the employee doesn't belong to.");
        OrganizationId = null;
    }

    public void AddItem(PropertyItem item)
    {
        if (this.AssignedItems.Any(i => i.Id == item.Id)) throw new BadRequest400Exception("This item is already assigned to another employee.");

        this.AssignedItems.Add(item);

        item.AssignEmployee(this);
    }
}