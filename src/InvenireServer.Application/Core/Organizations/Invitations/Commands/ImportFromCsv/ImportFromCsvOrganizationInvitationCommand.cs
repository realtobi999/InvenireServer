using InvenireServer.Domain.Entities.Common;

namespace InvenireServer.Application.Core.Organizations.Invitations.Commands.ImportFromCsv;

/// <summary>
/// Represents a request to import organization invitations from CSV.
/// </summary>
public class ImportFromCsvOrganizationInvitationCommand : IRequest
{
    public required Jwt Jwt { get; init; }
    public required Stream Stream { get; init; }
}
