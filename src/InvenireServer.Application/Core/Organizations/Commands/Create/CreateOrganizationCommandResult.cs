using InvenireServer.Domain.Entities.Organizations;

namespace InvenireServer.Application.Core.Organizations.Commands.Create;

/// <summary>
/// Represents the result of creating an organization.
/// </summary>
public record CreateOrganizationCommandResult
{
    public required Organization Organization { get; init; }
}