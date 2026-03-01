using InvenireServer.Domain.Entities.Common;

namespace InvenireServer.Application.Core.Organizations.Invitations.Commands.ImportFromJson;

/// <summary>
/// Represents a request to import organization invitations from JSON.
/// </summary>
public class ImportFromJsonOrganizationInvitationCommand : IRequest
{
    public required Jwt Jwt { get; init; }
    public required Stream Stream { get; init; }
}
