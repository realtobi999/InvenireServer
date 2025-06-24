using InvenireServer.Domain.Entities.Users;

namespace InvenireServer.Application.Cqrs.Admins.Commands.Register;

public record RegisterAdminCommandResult
{
    public required Admin Admin { get; init; }
    public required string Token { get; init; }
}
