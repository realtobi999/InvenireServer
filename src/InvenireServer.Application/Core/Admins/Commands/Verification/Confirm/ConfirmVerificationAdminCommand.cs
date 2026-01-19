using InvenireServer.Domain.Entities.Common;

namespace InvenireServer.Application.Core.Admins.Commands.Verification.Confirm;

/// <summary>
/// Represents a request to confirm a verification for an admin.
/// </summary>
public record ConfirmVerificationAdminCommand : IRequest
{
    public required Jwt Jwt { get; init; }
}