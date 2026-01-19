using System.Collections.ObjectModel;
using InvenireServer.Domain.Entities.Organizations;
using InvenireServer.Domain.Exceptions.Http;

namespace InvenireServer.Domain.Entities.Properties;

/// <summary>
/// Represents a property in the domain.
/// </summary>
public class Property
{
    // Core properties.

    public required Guid Id { get; set; }

    public required DateTimeOffset CreatedAt { get; set; }

    public required DateTimeOffset? LastUpdatedAt { get; set; }

    // Navigational properties.

    public Guid? OrganizationId { get; private set; }

    public Collection<PropertyItem> Items { get; } = [];

    public Collection<PropertyScan> Scans { get; } = [];

    public Collection<PropertySuggestion> Suggestions { get; } = [];

    // Methods.

    /// <summary>
    /// Assigns an organization to the property.
    /// </summary>
    /// <param name="organization">Organization to assign.</param>
    public void AssignOrganization(Organization organization)
    {
        if (OrganizationId is not null) throw new BadRequest400Exception("The property is already a part of another organization.");

        OrganizationId = organization.Id;
    }

    /// <summary>
    /// Adds a property item to the property.
    /// </summary>
    /// <param name="item">Item to add.</param>
    public void AddItem(PropertyItem item)
    {
        if (Items.Any(i => i.Id == item.Id)) throw new BadRequest400Exception("This item is already a part of this property.");

        Items.Add(item);

        item.AssignProperty(this);
    }

    /// <summary>
    /// Adds property items to the property.
    /// </summary>
    /// <param name="items">Items to add.</param>
    public void AddItems(IEnumerable<PropertyItem> items)
    {
        foreach (var item in items) AddItem(item);
    }

    /// <summary>
    /// Removes a property item from the property.
    /// </summary>
    /// <param name="item">Item to remove.</param>
    public void RemoveItem(PropertyItem item)
    {
        if (Items.All(i => i.Id != item.Id)) throw new BadRequest400Exception("This item is not a part of this property.");

        Items.Remove(item);

        item.UnassignProperty();
    }

    /// <summary>
    /// Removes property items from the property.
    /// </summary>
    /// <param name="items">Items to remove.</param>
    public void RemoveItems(IEnumerable<PropertyItem> items)
    {
        foreach (var item in items) RemoveItem(item);
    }
}
