namespace InvenireServer.Domain.Entities.Properties;

public class PropertyItem
{
    // Core properties.

    public required Guid Id { get; set; }

    public required string Name { get; set; }

    public required string Description { get; set; }

    public required DateTimeOffset CreatedAt { get; set; }

    public required DateTimeOffset? LastUpdatedAt { get; set; }

    // Navigational properties.

    public Guid? PropertyId { get; set; }

    public Guid? EmployeeId { get; set; }

}
