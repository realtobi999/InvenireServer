namespace InvenireServer.Application.Dtos.Admins.Email;

/// <summary>
/// Represents the information required to construct an administrator verification email.
/// </summary>
public record AdminVerificationEmailDto
{
    public required string AdminAddress { get; init; }
    public required string AdminFirstName { get; init; }
    public required string VerificationLink { get; init; }
}