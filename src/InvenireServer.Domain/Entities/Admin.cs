namespace InvenireServer.Domain.Entities;

/// <summary>
/// Represents an admin within the system.
/// </summary>
public class Admin
{
    // Core Properties.

    /// <summary>
    /// Unique identifier of the administrator.
    /// </summary>
    public required Guid Id { get; set; }

    /// <summary>
    /// Display name of the administrator.
    /// </summary>
    public required string Name { get; set; }

    /// <summary>
    /// Email address used for login and communication.
    /// </summary>
    public required string EmailAddress { get; set; }

    /// <summary>
    /// Hashed password for authentication.
    /// </summary>
    public required string Password { get; set; }

    /// <summary>
    /// Indicates whether the administrator has verified their email address.
    /// </summary>
    public required bool IsVerified { get; set; }

    /// <summary>
    /// Timestamp when the administrator account was created.
    /// </summary>
    public required DateTimeOffset CreatedAt { get; set; }

    /// <summary>
    /// Timestamp of the last update to the administrator's profile, if any.
    /// </summary>
    public required DateTimeOffset? LastUpdatedAt { get; set; }

    /// <summary>
    /// Timestamp of the last login, if any.
    /// </summary>
    public required DateTimeOffset? LastLoginAt { get; set; }

    // Navigational Properties.

    /// <summary>
    /// Identifier of the organization the administrator belongs to, if assigned.
    /// </summary>
    public Guid? OrganizationId { get; init; }

    // Constants

    /// <summary>
    /// Maximum allowed length for the administrator name.
    /// </summary>
    public const int MAX_NAME_LENGTH = 155;

    /// <summary>
    /// Maximum allowed length for the administrator email address.
    /// </summary>
    public const int MAX_EMAIL_ADDRESS_LENGTH = 155;

    /// <summary>
    /// Minimum required length for the administrator password.
    /// </summary>
    public const int MIN_PASSWORD_LENGTH = 8;

    /// <summary>
    /// Maximum allowed length for the administrator password.
    /// </summary>
    public const int MAX_PASSWORD_LENGTH = 155;
}