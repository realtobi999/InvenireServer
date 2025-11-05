using InvenireServer.Domain.Entities.Common;

namespace InvenireServer.Application.Core.Properties.Items.Commands.ImportFromJson;

public record ImportFromJsonPropertyItemsCommand : IRequest
{
    public required Jwt? Jwt { get; init; }
    public required Stream Stream { get; set; }
}
