using InvenireServer.Domain.Entities.Common;

namespace InvenireServer.Application.Core.Admins.Commands.Delete;

public record DeleteAdminCommand : IRequest
{
    public required Jwt Jwt { get; init; }
}
