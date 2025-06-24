using InvenireServer.Domain.Entities.Common;

namespace InvenireServer.Application.Core.Employees.Commands.Verification.Send;

public class SendVerificationEmployeeCommand : IRequest
{
    public required Jwt Jwt { get; set; }
    public required string FrontendBaseUrl { get; set; }
}
