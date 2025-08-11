using InvenireServer.Domain.Entities.Common;

namespace InvenireServer.Application.Core.Organizations.Commands.Delete;

public record DeleteOrganizationCommand : IRequest
{
    public required Jwt Jwt { get; init; }
}
