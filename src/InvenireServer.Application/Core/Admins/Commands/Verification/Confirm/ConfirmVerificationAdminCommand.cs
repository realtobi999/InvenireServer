using InvenireServer.Domain.Entities.Common;

namespace InvenireServer.Application.Cqrs.Admins.Commands.Verification.Confirm;

public class ConfirmVerificationAdminCommand : IRequest<Unit>
{
    public required Jwt Jwt { get; set; }
}
