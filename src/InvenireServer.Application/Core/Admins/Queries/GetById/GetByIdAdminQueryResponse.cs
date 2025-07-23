using InvenireServer.Application.Dtos.Admins;

namespace InvenireServer.Application.Core.Admins.Queries.GetById;

public record GetByIdAdminQueryResponse
{
    public required AdminDto AdminDto { get; set; }
}
