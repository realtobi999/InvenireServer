namespace InvenireServer.Domain.Entities;

public class Organization
{
    // Constants.

    public const int MAX_NAME_LENGTH = 155;

    // Core properties.

    public required Guid Id { get; set; }

    public required string Name { get; set; }

    public required DateTimeOffset CreatedAt { get; set; }

    public required DateTimeOffset? LastUpdatedAt { get; set; }

    // Navigational properties.

    public Admin? Admin { get; set; }

    public ICollection<Employee>? Employees { get; set; }
}
