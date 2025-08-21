using InvenireServer.Domain.Entities.Common;

namespace InvenireServer.Application.Core.Employees.Commands.Verification.Send;

public record SendVerificationEmployeeCommand : IRequest
{
    public required Jwt Jwt { get; init; }
    public required string FrontendBaseAddress { get; init; }
}