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

    public Employee? Employee { get; private set; }

    // Methods.

    public void AssignOrganization(Organization organization)
    {
        if (OrganizationId is not null) throw new BadRequest400Exception("This invitation is already part of a other organization");

        OrganizationId = organization.Id;
    }

    public void UnassignOrganization()
    {
        if (OrganizationId is null) throw new BadRequest400Exception("This invitation is not part of a any organization");

        OrganizationId = null;
    }

    public void AssignEmployee(Employee employee)
    {
        if (Employee is not null) throw new BadRequest400Exception("This invitation does already have a employee assigned");

        Employee = employee;
    }
}