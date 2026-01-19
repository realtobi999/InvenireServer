using InvenireServer.Domain.Entities.Common;

namespace InvenireServer.Application.Core.Admins.Commands.Delete;

/// <summary>
/// Represents a request to delete an admin.
/// </summary>
public record DeleteAdminCommand : IRequest
{
    public required Jwt Jwt { get; init; }
}
