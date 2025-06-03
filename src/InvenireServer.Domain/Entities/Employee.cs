namespace InvenireServer.Domain.Entities;

/// <summary>
/// Represents an employee within the system.
/// </summary>
public class Employee
{
    // Core Properties.

    /// <summary>
    /// Unique identifier of the employee.
    /// </summary>
    public required Guid Id { get; init; }

    /// <summary>
    /// Full name of the employee.
    /// </summary>
    public required string Name { get; set; }

    /// <summary>
    /// Hashed password used to authenticate the employee.
    /// </summary>
    public required string Password { get; set; }

    /// <summary>
    /// Email address associated with the employee's account.
    /// </summary>
    public required string EmailAddress { get; set; }

    /// <summary>
    /// Indicates whether the employee's email address has been verified.
    /// </summary>
    public required bool IsVerified { get; set; }

    /// <summary>
    /// Date and time of the most recent update to the employee record.
    /// Null if the record has never been updated.
    /// </summary>
    public required DateTimeOffset? UpdatedAt { get; set; }

    /// <summary>
    /// Date and time when the employee record was created.
    /// </summary>
    public required DateTimeOffset CreatedAt { get; set; }

    /// <summary>
    /// Date and time of the employee's last successful login. Null if never logged in.
    /// </summary>
    public required DateTimeOffset? LastLoginAt { get; set; }

    // Navigational Properties.

    /// <summary>
    /// Optional identifier of the organization the employee belongs to.
    /// </summary>
    public Guid? OrganizationId { get; init; }

    // Constants.

    /// <summary>
    /// Maximum number of characters allowed for the employee's name.
    /// </summary>
    public const int MAX_NAME_LENGTH = 155;

    /// <summary>
    /// Maximum number of characters allowed for the employee's email address.
    /// </summary>
    public const int MAX_EMAIL_ADDRESS_LENGTH = 155;

    /// <summary>
    /// Minimum number of characters required for the employee's password.
    /// </summary>
    public const int MIN_PASSWORD_LENGTH = 8;

    /// <summary>
    /// Maximum number of characters allowed for the employee's password.
    /// </summary>
    public const int MAX_PASSWORD_LENGTH = 155;

}
