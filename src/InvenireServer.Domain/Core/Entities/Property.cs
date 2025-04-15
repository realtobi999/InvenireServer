namespace InvenireServer.Domain.Core.Entities;

public class Property
{
    public required Guid Id { get; init; }
    public required List<PropertyGroup> Groups { get; set; } = [];

    // Navigational Properties.
    public Guid OrganizationId { get; init; }
}
