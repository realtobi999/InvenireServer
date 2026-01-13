namespace InvenireServer.Application.Dtos.Admins.Email;

/// <summary>
/// Represents the information required to construct an administrator password recovery email.
/// </summary>
public record AdminRecoveryEmailDto
{
    public required string AdminAddress { get; init; }
    public required string RecoveryLink { get; init; }
}
