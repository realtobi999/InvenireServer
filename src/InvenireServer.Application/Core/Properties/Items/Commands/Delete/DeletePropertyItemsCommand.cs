using InvenireServer.Domain.Entities.Common;

namespace InvenireServer.Application.Core.Properties.Items.Commands.Delete;

public record DeletePropertyItemsCommand : IRequest
{
    public required List<Guid> Ids { get; init; }
    public required Jwt? Jwt { get; init; }
}