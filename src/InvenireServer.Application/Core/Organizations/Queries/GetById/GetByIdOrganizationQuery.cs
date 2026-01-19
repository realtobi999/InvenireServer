using InvenireServer.Application.Dtos.Organizations;

namespace InvenireServer.Application.Core.Organizations.Queries.GetById;

/// <summary>
/// Represents a query to get an organization by ID.
/// </summary>
public record GetByIdOrganizationQuery : IRequest<OrganizationDto>
{
    public required Guid OrganizationId { get; set; }
}
