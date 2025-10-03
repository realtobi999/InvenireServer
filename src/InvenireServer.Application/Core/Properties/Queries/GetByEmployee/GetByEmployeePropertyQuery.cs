using InvenireServer.Domain.Entities.Common;
using InvenireServer.Application.Dtos.Properties;

namespace InvenireServer.Application.Core.Properties.Queries.GetByEmployee;

public record GetByEmployeePropertyQuery : IRequest<PropertyDto>
{
    public required Jwt Jwt { get; init; }
}
