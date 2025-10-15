namespace InvenireServer.Application.Dtos.Admins.Email;

public record AdminVerificationEmailDto
{
    public required string AdminAddress { get; init; }
    public required string AdminFirstName { get; init; }
    public required string VerificationLink { get; init; }
}