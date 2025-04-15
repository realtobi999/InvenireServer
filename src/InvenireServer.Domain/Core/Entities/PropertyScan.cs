namespace InvenireServer.Domain.Core.Entities;

public class PropertyScan
{
    public required Guid Id { get; init; }
    public required List<PropertyItem> ScannedItems { get; set; } = [];

    // Navigational Properties.
    public Guid PropertyId { get; init; }
    public Guid OrganizationId { get; init; }
}
