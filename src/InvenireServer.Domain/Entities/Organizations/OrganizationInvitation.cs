using InvenireServer.Domain.Entities.Users;

namespace InvenireServer.Domain.Entities.Organizations;

public class OrganizationInvitation
{
    // Constants.

    public const int MAX_DESCRIPTION_LENGTH = 555;

    // Core properties.

    public required Guid Id { get; set; }

    public required string? Description { get; set; }

    public required DateTimeOffset CreatedAt { get; set; }

    public required DateTimeOffset? LastUpdatedAt { get; set; }

    // Navigational properties.

    public Guid? OrganizationId { get; set; }

    public Employee? Employee { get; set; }
}