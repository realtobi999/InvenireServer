using InvenireServer.Domain.Entities.Users;

namespace InvenireServer.Application.Core.Admins.Commands.Register;

public record RegisterAdminCommandResult
{
    public required Admin Admin { get; init; }
    public required string Token { get; init; }
}