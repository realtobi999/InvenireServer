namespace InvenireServer.Domain.Core.Entities;

public class PropertyGroup
{
    public required Guid Id { get; init; }
    public required List<PropertyItem> Items { get; set; } = [];
    public required List<PropertyGroup> Subs { get; set; } = [];

    // Navigational Properties.
    public Guid PropertyId { get; init; }
    public Guid? ParentGroupId { get; init; }
}