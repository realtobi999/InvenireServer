using InvenireServer.Domain.Entities.Common;

namespace InvenireServer.Application.Core.Admins.Commands.Verification.Send;

public record SendVerificationAdminCommand : IRequest
{
    public required Jwt Jwt { get; set; }
    public required string FrontendBaseUrl { get; set; }
}