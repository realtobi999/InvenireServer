using InvenireServer.Domain.Entities.Common;

namespace InvenireServer.Application.Core.Properties.Items.Commands.ImportFromJson;

/// <summary>
/// Represents a request to import property items from JSON.
/// </summary>
public record ImportFromJsonPropertyItemsCommand : IRequest
{
    public required Jwt? Jwt { get; init; }
    public required Stream Stream { get; set; }
}
