using InvenireServer.Domain.Entities.Common;

namespace InvenireServer.Application.Core.Admins.Commands.Verification.Confirm;

public class ConfirmVerificationAdminCommand : IRequest
{
    public required Jwt Jwt { get; set; }
}