using System.Collections.ObjectModel;
using InvenireServer.Domain.Entities.Organizations;
using InvenireServer.Domain.Exceptions.Http;

namespace InvenireServer.Domain.Entities.Properties;

public class Property
{
    // Core properties.

    public required Guid Id { get; set; }

    public required DateTimeOffset CreatedAt { get; set; }

    public required DateTimeOffset? LastUpdatedAt { get; set; }

    // Navigational properties.

    public Guid? OrganizationId { get; set; }

    public Collection<PropertyItem> Items { get; set; } = [];

    // Methods.

    public void AssignOrganization(Organization organization)
    {
        if (OrganizationId is not null) throw new BadRequest400Exception("This property is already a part of a another organization");

        OrganizationId = organization.Id;
    }

    public void AddItem(PropertyItem item)
    {
        if (Items.Any(i => i.Id == item.Id)) throw new BadRequest400Exception("This item is already a part of this property.");

        Items.Add(item);

        item.AssignProperty(this);
    }
}