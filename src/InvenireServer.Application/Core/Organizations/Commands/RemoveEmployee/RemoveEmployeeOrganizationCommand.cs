using InvenireServer.Domain.Entities.Common;

namespace InvenireServer.Application.Core.Organizations.Commands.RemoveEmployee;

public record RemoveEmployeeOrganizationCommand : IRequest
{
    public required Jwt Jwt { get; init; }
    public required Guid EmployeeId { get; init; }
}
