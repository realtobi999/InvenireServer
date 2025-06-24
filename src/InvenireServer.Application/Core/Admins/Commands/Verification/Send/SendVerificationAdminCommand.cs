using InvenireServer.Domain.Entities.Common;

namespace InvenireServer.Application.Cqrs.Admins.Commands.Verification.Send;

public record SendVerificationAdminCommand : IRequest<Unit>
{
    public required Jwt Jwt { get; set; }
    public required string FrontendBaseUrl { get; set; }
}
