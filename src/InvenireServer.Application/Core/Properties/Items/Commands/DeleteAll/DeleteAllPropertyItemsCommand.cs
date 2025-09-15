using InvenireServer.Domain.Entities.Common;

namespace InvenireServer.Application.Core.Properties.Items.Commands.DeleteAll;

public record DeleteAllPropertyItemsCommand : IRequest
{
    public required Jwt? Jwt { get; init; }
}
