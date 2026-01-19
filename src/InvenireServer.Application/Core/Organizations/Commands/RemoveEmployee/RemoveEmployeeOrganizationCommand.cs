using InvenireServer.Domain.Entities.Common;

namespace InvenireServer.Application.Core.Organizations.Commands.RemoveEmployee;

/// <summary>
/// Represents a request to remove an employee from an organization.
/// </summary>
public record RemoveEmployeeOrganizationCommand : IRequest
{
    public required Jwt Jwt { get; init; }
    public required Guid EmployeeId { get; init; }
}
