using InvenireServer.Domain.Entities.Common;

namespace InvenireServer.Application.Core.Properties.Items.Queries.ExportToJson;

public record ExportToJsonPropertyItemQuery : IRequest<Stream>
{
    public required Jwt Jwt { get; init; }
}
