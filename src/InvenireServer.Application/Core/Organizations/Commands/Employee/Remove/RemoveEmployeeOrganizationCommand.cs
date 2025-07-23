using InvenireServer.Domain.Entities.Common;

namespace InvenireServer.Application.Core.Organizations.Commands.Employee.Remove;

public class RemoveEmployeeOrganizationCommand : IRequest
{
    public required Guid EmployeeId { get; set; }
    public required Jwt Jwt { get; set; }
}
