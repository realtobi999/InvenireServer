using InvenireServer.Domain.Entities.Common;

namespace InvenireServer.Application.Core.Organizations.Commands.Remove;

public class RemoveOrganizationEmployeeCommand : IRequest
{
    public required Guid EmployeeId { get; set; }
    public required Jwt Jwt { get; set; }
}
