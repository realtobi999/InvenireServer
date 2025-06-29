using InvenireServer.Domain.Entities.Organizations;
using InvenireServer.Domain.Exceptions.Http;

namespace InvenireServer.Domain.Entities.Users;

public class Employee
{
    // Constants.

    public const int MAX_NAME_LENGTH = 155;

    public const int MAX_EMAIL_ADDRESS_LENGTH = 155;

    public const int MIN_PASSWORD_LENGTH = 8;

    public const int MAX_PASSWORD_LENGTH = 155;

    public static readonly TimeSpan INACTIVE_THRESHOLD = TimeSpan.FromDays(14);

    // Core Properties.

    public required Guid Id { get; init; }

    public required string Name { get; set; }

    public required string Password { get; set; }

    public required string EmailAddress { get; set; }

    public required bool IsVerified { get; set; }

    public required DateTimeOffset CreatedAt { get; set; }

    public required DateTimeOffset? LastUpdatedAt { get; set; }

    public required DateTimeOffset? LastLoginAt { get; set; }

    // Navigational Properties.

    public Guid? OrganizationId { get; set; }

    // Methods.

    public void Verify()
    {
        if (IsVerified) throw new BadRequest400Exception("Employee is already verified.");

        IsVerified = true;
    }

    public void AssignOrganization(Organization organization)
    {
        if (OrganizationId is not null) throw new BadRequest400Exception("Employee is already part of a organization");
        OrganizationId = organization.Id;
    }
}