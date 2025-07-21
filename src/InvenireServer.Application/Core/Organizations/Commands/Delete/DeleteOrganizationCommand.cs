using InvenireServer.Domain.Entities.Common;

namespace InvenireServer.Application.Core.Organizations.Commands.Delete;

public class DeleteOrganizationCommand : IRequest
{
    public required Jwt Jwt { get; set; }
}
