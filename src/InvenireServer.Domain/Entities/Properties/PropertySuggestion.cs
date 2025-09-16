namespace InvenireServer.Domain.Entities.Properties;

public class PropertySuggestion
{
    // Constants.

    public const int MAX_NAME_LENGTH = 155;

    public const int MAX_DESCRIPTION_LENGTH = 555;

    public const int MAX_FEEDBACK_LENGTH = 555;

    public readonly static TimeSpan EXPIRATION_TIME = TimeSpan.FromDays(182);

    // Core properties.

    public required Guid Id { get; set; }

    public required string Name { get; set; }

    public required string? Description { get; set; }

    public required string? Feedback { get; set; }

    public required string PayloadString { get; set; }

    public required PropertySuggestionStatus Status { get; set; }

    public required DateTimeOffset CreatedAt { get; set; }

    public required DateTimeOffset? ResolvedAt { get; set; }

    public required DateTimeOffset? LastUpdatedAt { get; set; }

    // Navigational properties.

    public Guid? EmployeeId { get; set; }

    public Guid? PropertyId { get; set; }
}

public enum PropertySuggestionStatus
{
    APPROVED = 0,
    PENDING = 1,
    DECLINED = 2,
}
