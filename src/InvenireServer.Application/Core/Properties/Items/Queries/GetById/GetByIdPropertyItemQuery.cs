using InvenireServer.Domain.Entities.Common;
using InvenireServer.Application.Dtos.Properties;

namespace InvenireServer.Application.Core.Properties.Items.Queries.GetById;

public record GetByIdPropertyItemQuery : IRequest<PropertyItemDto>
{
    public required Jwt Jwt { get; init; }
    public required Guid ItemId { get; init; }
}
