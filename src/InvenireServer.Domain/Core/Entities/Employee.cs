namespace InvenireServer.Domain.Core.Entities;

public class Employee
{
    public required Guid Id { get; init; }
    public required string Name { get; set; }
    public required string EmailAddress { get; set; }
    public required string Password { get; set; }
    public required DateTimeOffset? UpdatedAt { get; set; }
    public required DateTimeOffset CreatedAt { get; set; }

    // Navigational Properties.
    public Guid? OrganizationId { get; init; }

    // Constants.
    public const int MAX_NAME_LENGTH = 155;
    public const int MIN_PASSWORD_LENGTH = 8;
    public const int MAX_PASSWORD_LENGTH = 155;
    public const int MAX_EMAIL_ADDRESS_LENGTH = 155;
}