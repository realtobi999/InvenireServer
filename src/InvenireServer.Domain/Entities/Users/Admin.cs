using InvenireServer.Domain.Entities.Organizations;
using InvenireServer.Domain.Exceptions.Http;

namespace InvenireServer.Domain.Entities.Users;

public class Admin
{
    // Constants

    public const int MIN_NAME_LENGTH = 3;

    public const int MAX_NAME_LENGTH = 15;

    public const int MAX_EMAIL_ADDRESS_LENGTH = 155;

    public const int MIN_PASSWORD_LENGTH = 8;

    public const int MAX_PASSWORD_LENGTH = 155;

    public static readonly TimeSpan INACTIVE_THRESHOLD = TimeSpan.FromDays(7);

    // Core Properties.

    public required Guid Id { get; set; }

    public required string FirstName { get; set; }

    public required string LastName { get; set; }

    public required string EmailAddress { get; set; }

    public required string Password { get; set; }

    public required bool IsVerified { get; set; }

    public required DateTimeOffset CreatedAt { get; set; }

    public required DateTimeOffset? LastUpdatedAt { get; set; }

    public required DateTimeOffset? LastLoginAt { get; set; }

    // Navigational Properties.

    public Guid? OrganizationId { get; private set; }

    // Methods.

    public void Verify()
    {
        if (IsVerified) throw new Conflict409Exception("The admin's verification status is already confirmed.");

        IsVerified = true;
    }

    public void AssignOrganization(Organization organization)
    {
        if (OrganizationId is not null) throw new Conflict409Exception("The admin is already a owner of another organization.");

        OrganizationId = organization.Id;
    }
}