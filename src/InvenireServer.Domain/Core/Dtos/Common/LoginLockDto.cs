namespace InvenireServer.Domain.Core.Dtos.Common;

public record LoginLockDto
{
    public required bool IsSet { get; set; }
    public required DateTimeOffset? ExpirationDate { get; set; }
}
