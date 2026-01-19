using InvenireServer.Domain.Entities.Common;

namespace InvenireServer.Application.Core.Admins.Commands.Verification.Send;

/// <summary>
/// Represents a request to send a verification for an admin.
/// </summary>
public record SendVerificationAdminCommand : IRequest
{
    public required Jwt Jwt { get; init; }
    public required string FrontendBaseAddress { get; init; }
}