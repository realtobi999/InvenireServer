using System.Linq.Expressions;
using InvenireServer.Application.Dtos.Admins;
using InvenireServer.Application.Interfaces.Managers;
using InvenireServer.Domain.Entities.Common.Queries;
using InvenireServer.Domain.Entities.Users;
using InvenireServer.Domain.Exceptions.Http;

namespace InvenireServer.Application.Core.Admins.Queries.GetByJwt;

public class GetByJwtAdminQueryHandler : IRequestHandler<GetByJwtAdminQuery, AdminDto>
{
    private readonly IRepositoryManager _services;

    public GetByJwtAdminQueryHandler(IRepositoryManager services)
    {
        _services = services;
    }

    public async Task<AdminDto> Handle(GetByJwtAdminQuery request, CancellationToken ct)
    {
        return await _services.Admins.GetAsync(request.Jwt, new QueryOptions<Admin, AdminDto>
        {
            Selector = AdminDtoSelector
        }) ?? throw new NotFound404Exception("The admin was not found in the system.");
    }

    private static Expression<Func<Admin, AdminDto>> AdminDtoSelector
    {
        get
        {
            return a => new AdminDto
            {
                Id = a.Id,
                OrganizationId = a.OrganizationId,
                FirstName = a.FirstName,
                LastName = a.LastName,
                FullName = $"{a.FirstName} {a.LastName}",
                EmailAddress = a.EmailAddress,
                CreatedAt = a.CreatedAt,
                LastUpdatedAt = a.LastUpdatedAt
            };
        }
    }
}
