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

    public Guid? OrganizationId { get; private set; }

    public Collection<PropertyItem> AssignedItems { get; } = [];

    public Collection<PropertySuggestion> Suggestions { get; } = [];

    // Methods.

    public void Verify()
    {
        if (IsVerified) throw new BadRequest400Exception("The employees's verification status is already confirmed.");

        IsVerified = true;
    }

    public void AssignOrganization(Organization organization)
    {
        if (OrganizationId is not null) throw new BadRequest400Exception("This employee is already part of a another organization");

        OrganizationId = organization.Id;
    }

    public void UnassignOrganization()
    {
        if (OrganizationId is null) throw new BadRequest400Exception("This employee is not part of a any organization");

        OrganizationId = null;
    }

    public void AddItem(PropertyItem item)
    {
        if (AssignedItems.Any(i => i.Id == item.Id)) throw new BadRequest400Exception("This item is already assigned to this employee.");

        AssignedItems.Add(item);

        item.AssignEmployee(this);
    }

    public void AddItems(IEnumerable<PropertyItem> items)
    {
        foreach (var item in items) AddItem(item);
    }

    public void RemoveItem(PropertyItem item)
    {
        if (AssignedItems.All(i => i.Id != item.Id)) throw new BadRequest400Exception("This item is not assigned to this employee.");

        AssignedItems.Remove(item);

        item.UnassignEmployee();
    }

    public void AddSuggestion(PropertySuggestion suggestion)
    {
        if (Suggestions.Any(s => s.Id == suggestion.Id)) throw new BadRequest400Exception("This suggestion was already assigned to this employee.");

        Suggestions.Add(suggestion);
    }
}