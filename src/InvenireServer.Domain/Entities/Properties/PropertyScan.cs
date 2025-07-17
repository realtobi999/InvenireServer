namespace InvenireServer.Domain.Entities.Properties;

public class PropertyScan
{
    // Constants.

    public const int MAX_NAME_LENGTH = 155;

    public const int MAX_DESCRIPTION_LENGTH = 555;

    // Core properties.

    public required Guid Id { get; set; }

    public required string Name { get; set; }

    public required string? Description { get; set; }

    public required PropertyScanStatus Status { get; set; }

    public required DateTimeOffset CreatedAt { get; init; }

    public required DateTimeOffset? CompletedAt { get; set; }

    public required DateTimeOffset? LastUpdatedAt { get; set; }

    // Navigational properties.

    public Guid? PropertyId { get; set; }

    public ICollection<PropertyItem> ScannedItems { get; set; } = [];
}

public enum PropertyScanStatus
{
    COMPLETED,
    IN_PROGRESS,
}