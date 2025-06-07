namespace InvenireServer.Application.Dtos.Admins.Email;

public record class AdminVerificationEmailDto
{
    public required string AdminAddress { get; init; }

    public required string AdminName { get; init; }

    public required string VerificationLink { get; init; }
}