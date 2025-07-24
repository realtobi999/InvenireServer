using InvenireServer.Application.Dtos.Admins;

namespace InvenireServer.Application.Core.Admins.Queries.GetById;

public record GetByIdAdminQuery : IRequest<AdminDto>
{
    public required Guid AdminId { get; set; }
}
