using System.Collections.ObjectModel;
using InvenireServer.Domain.Entities.Organizations;
using InvenireServer.Domain.Entities.Properties;
using InvenireServer.Domain.Exceptions.Http;

namespace InvenireServer.Domain.Entities.Users;

/// <summary>
/// Represents an employee in the domain.
/// </summary>
public class Employee
{
    // Constants.

    public const int MIN_NAME_LENGTH = 3;

    public const int MAX_NAME_LENGTH = 15;

    public const int MAX_EMAIL_ADDRESS_LENGTH = 155;

    public const int MIN_PASSWORD_LENGTH = 8;

    public const int MAX_PASSWORD_LENGTH = 155;

    public static readonly TimeSpan INACTIVE_THRESHOLD = TimeSpan.FromDays(14);

    // Core Properties.

    public required Guid Id { get; init; }

    public required string FirstName { get; set; }

    public required string LastName { get; set; }

    public required string Password { get; set; }

    public required string EmailAddress { get; set; }

    public required bool IsVerified { get; set; }

    public required DateTimeOffset CreatedAt { get; set; }

    public required DateTimeOffset? LastUpdatedAt { get; set; }

    public required DateTimeOffset? LastLoginAt { get; set; }

    // Navigational Properties.

    public Guid? OrganizationId { get; set; }

    public Collection<PropertyItem> AssignedItems { get; } = [];

    public Collection<PropertySuggestion> Suggestions { get; } = [];

    // Methods.

    /// <summary>
    /// Marks the employee as verified.
    /// </summary>
    public void Verify()
    {
        if (IsVerified) throw new BadRequest400Exception("The employees's verification status is already confirmed.");

        IsVerified = true;
    }

    /// <summary>
    /// Assigns an organization to the employee.
    /// </summary>
    /// <param name="organization">Organization to assign.</param>
    public void AssignOrganization(Organization organization)
    {
        if (OrganizationId is not null) throw new BadRequest400Exception("The employee is already part of another organization");

        OrganizationId = organization.Id;
    }

    /// <summary>
    /// Unassigns the employee from the organization.
    /// </summary>
    public void UnassignOrganization()
    {
        if (OrganizationId is null) throw new BadRequest400Exception("The employee is not part of any organization");

        OrganizationId = null;
    }

    /// <summary>
    /// Adds a property item to the employee.
    /// </summary>
    /// <param name="item">Item to add.</param>
    public void AddItem(PropertyItem item)
    {
        AssignedItems.Add(item);

        item.AssignEmployee(this);
    }

    /// <summary>
    /// Adds property items to the employee.
    /// </summary>
    /// <param name="items">Items to add.</param>
    public void AddItems(IEnumerable<PropertyItem> items)
    {
        foreach (var item in items) AddItem(item);
    }

    /// <summary>
    /// Removes a property item from the employee.
    /// </summary>
    /// <param name="item">Item to remove.</param>
    public void RemoveItem(PropertyItem item)
    {
        if (AssignedItems.All(i => i.Id != item.Id)) throw new BadRequest400Exception("This item is not assigned to this employee.");

        AssignedItems.Remove(item);

        item.UnassignEmployee();
    }

    /// <summary>
    /// Adds a property suggestion to the employee.
    /// </summary>
    /// <param name="suggestion">Suggestion to add.</param>
    public void AddSuggestion(PropertySuggestion suggestion)
    {
        if (Suggestions.Any(s => s.Id == suggestion.Id)) throw new BadRequest400Exception("This suggestion was already assigned to this employee.");

        Suggestions.Add(suggestion);
    }
}
