using InvenireServer.Domain.Entities.Common;
using InvenireServer.Domain.Entities.Users;

namespace InvenireServer.Application.Core.Admins.Commands.Register;

public record RegisterAdminCommandResult
{
    public required Jwt Token { get; init; }
    public required string TokenString { get; init; }
    public required Admin Admin { get; init; }
}
