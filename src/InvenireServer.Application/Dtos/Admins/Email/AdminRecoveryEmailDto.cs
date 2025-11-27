namespace InvenireServer.Application.Dtos.Admins.Email;

public record AdminRecoveryEmailDto
{
    public required string AdminAddress { get; init; }
    public required string AdminFirstName { get; init; }
    public required string RecoveryLink { get; init; }
}
