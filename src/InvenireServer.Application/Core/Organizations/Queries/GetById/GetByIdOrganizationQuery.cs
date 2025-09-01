using InvenireServer.Application.Dtos.Organizations;

namespace InvenireServer.Application.Core.Organizations.Queries.GetById;

public record GetByIdOrganizationQuery : IRequest<OrganizationDto>
{
    public required Guid OrganizationId { get; set; }
}
