using InvenireServer.Domain.Entities.Organizations;

namespace InvenireServer.Application.Core.Organizations.Commands.Create;

public record class CreateOrganizationCommandResult
{
    public required Organization Organization { get; set; }
}
