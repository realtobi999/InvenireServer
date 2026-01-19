using InvenireServer.Domain.Entities.Common;
using InvenireServer.Application.Dtos.Properties;

namespace InvenireServer.Application.Core.Properties.Items.Queries.GetById;

/// <summary>
/// Represents a query to get a property item by ID.
/// </summary>
public record GetByIdPropertyItemQuery : IRequest<PropertyItemDto>
{
    public required Jwt Jwt { get; init; }
    public required Guid ItemId { get; init; }
}
