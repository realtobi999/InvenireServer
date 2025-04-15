namespace InvenireServer.Domain.Core.Entities;

public class Employee
{
    public required Guid Id { get; init; }
    public required List<PropertyItem> AssignedItems { get; set; } = [];

    // Navigational Properties.
    public Guid OrganizationId { get; init; }
}
