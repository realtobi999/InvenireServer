namespace InvenireServer.Application.Cqrs.Admins.Commands.Login;

public record LoginAdminCommandResult
{
    public required string Token { get; init; }
}
