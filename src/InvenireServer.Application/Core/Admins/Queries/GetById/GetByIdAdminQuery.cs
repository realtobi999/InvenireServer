namespace InvenireServer.Application.Core.Admins.Queries.GetById;

public record GetByIdAdminQuery : IRequest<GetByIdAdminQueryResponse>
{
    public required Guid AdminId { get; set; }
}
