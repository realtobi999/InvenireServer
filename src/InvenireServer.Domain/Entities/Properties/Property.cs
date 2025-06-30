using System.Collections.ObjectModel;

namespace InvenireServer.Domain.Entities.Properties;

public class Property
{
    // Core properties.

    public Guid Id { get; set; }

    public required DateTimeOffset CreatedAt { get; set; }

    public required DateTimeOffset? LastUpdatedAt { get; set; }

    // Navigational properties.

    public Guid? OrganizationId { get; set; }

    public Collection<PropertyItem> Items { get; set; } = [];
}
