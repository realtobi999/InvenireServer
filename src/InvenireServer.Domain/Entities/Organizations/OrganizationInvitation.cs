using InvenireServer.Domain.Entities.Users;
using InvenireServer.Domain.Exceptions.Http;

namespace InvenireServer.Domain.Entities.Organizations;

public class OrganizationInvitation
{
    // Constants.

    public const int MAX_DESCRIPTION_LENGTH = 555;

    public static readonly TimeSpan EXPIRATION_TIME = TimeSpan.FromDays(30);

    // Core properties.

    public required Guid Id { get; set; }

    public required string? Description { get; set; }

    public required DateTimeOffset CreatedAt { get; set; }

    public required DateTimeOffset? LastUpdatedAt { get; set; }

    // Navigational properties.

    public Guid? OrganizationId { get; private set; }

    public Employee? Employee { get; set; }

    // Methods.

    public void AssignOrganization(Organization organization)
    {
        OrganizationId = organization.Id;
    }

    public void UnassignOrganization()
    {
        OrganizationId = null;
    }
}