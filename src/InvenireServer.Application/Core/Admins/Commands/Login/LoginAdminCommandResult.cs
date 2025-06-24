namespace InvenireServer.Application.Core.Admins.Commands.Login;

public record LoginAdminCommandResult
{
    public required string Token { get; init; }
}
