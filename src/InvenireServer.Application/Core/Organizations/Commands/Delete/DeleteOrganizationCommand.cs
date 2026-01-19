using InvenireServer.Domain.Entities.Common;

namespace InvenireServer.Application.Core.Organizations.Commands.Delete;

/// <summary>
/// Represents a request to delete an organization.
/// </summary>
public record DeleteOrganizationCommand : IRequest
{
    public required Jwt Jwt { get; init; }
}
