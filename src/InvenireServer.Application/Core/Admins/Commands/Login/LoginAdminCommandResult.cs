using InvenireServer.Domain.Entities.Users;
using InvenireServer.Domain.Entities.Common;

namespace InvenireServer.Application.Core.Admins.Commands.Login;

public record LoginAdminCommandResult
{
    public required Jwt Token { get; init; }
    public required string TokenString { get; init; }
}

