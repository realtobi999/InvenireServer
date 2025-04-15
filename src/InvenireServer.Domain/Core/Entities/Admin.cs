namespace InvenireServer.Domain.Core.Entities;

public class Admin
{
    public required Guid Id { get; init; }

    // Navigational Properties.
    public Guid OrganizationId { get; init; }
}
