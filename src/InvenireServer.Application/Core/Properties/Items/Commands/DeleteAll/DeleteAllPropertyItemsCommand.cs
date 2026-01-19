using InvenireServer.Domain.Entities.Common;

namespace InvenireServer.Application.Core.Properties.Items.Commands.DeleteAll;

/// <summary>
/// Represents a request to delete all property items.
/// </summary>
public record DeleteAllPropertyItemsCommand : IRequest
{
    public required Jwt? Jwt { get; init; }
}
