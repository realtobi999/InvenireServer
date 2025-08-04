using InvenireServer.Domain.Entities.Common;
using InvenireServer.Application.Dtos.Properties;

namespace InvenireServer.Application.Core.Properties.Queries.GetByAdmin;

public record GetByAdminPropertyQuery : IRequest<PropertyDto>
{
    public required Jwt Jwt { get; set; }
}
