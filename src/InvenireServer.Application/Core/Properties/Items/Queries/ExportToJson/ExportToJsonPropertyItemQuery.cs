using InvenireServer.Domain.Entities.Common;

namespace InvenireServer.Application.Core.Properties.Items.Queries.ExportToJson;

/// <summary>
/// Represents a query to export property items to JSON.
/// </summary>
public record ExportToJsonPropertyItemQuery : IRequest<Stream>
{
    public required Jwt Jwt { get; init; }
}
