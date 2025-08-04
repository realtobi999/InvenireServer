using InvenireServer.Application.Dtos.Properties;
using InvenireServer.Domain.Entities.Common;

namespace InvenireServer.Application.Core.Properties.Items.Queries;

public class IndexForAdminPropertyItemQuery : IRequest<IEnumerable<PropertyItemDto>>
{
    public required Jwt Jwt { get; set; }
    public required PaginationParameters Pagination { get; set; }
}
