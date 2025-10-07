using System.Linq.Expressions;
using InvenireServer.Domain.Exceptions.Http;
using InvenireServer.Domain.Entities.Properties;
using InvenireServer.Application.Dtos.Properties;
using InvenireServer.Domain.Entities.Common.Queries;
using InvenireServer.Application.Interfaces.Managers;

namespace InvenireServer.Application.Core.Properties.Queries.GetByEmployee;

public class GetByEmployeePropertyQueryHandler : IRequestHandler<GetByEmployeePropertyQuery, PropertyDto>
{
    private readonly IRepositoryManager _repositories;

    public GetByEmployeePropertyQueryHandler(IRepositoryManager repositories)
    {
        _repositories = repositories;
    }

    public async Task<PropertyDto> Handle(GetByEmployeePropertyQuery request, CancellationToken ct)
    {
        var employee = await _repositories.Employees.GetAsync(request.Jwt) ?? throw new NotFound404Exception("The employee was not found in the system.");
        var organization = await _repositories.Organizations.GetForAsync(employee) ?? throw new BadRequest400Exception("The employee isn't part of any organization.");

        return await _repositories.Properties.GetAsync(new QueryOptions<Property, PropertyDto>
        {
            Selector = Selector,
            Filtering = new QueryFilteringOptions<Property>
            {
                Filters =
                [
                    p => p.OrganizationId == organization.Id,
                ]
            }
        }) ?? throw new BadRequest400Exception("The organization doesn't have a property.");
    }

    private static Expression<Func<Property, PropertyDto>> Selector
    {
        get
        {
            return p => new PropertyDto
            {
                Id = p.Id,
                OrganizationId = p.OrganizationId,
                CreatedAt = p.CreatedAt,
                LastUpdatedAt = p.LastUpdatedAt,
                ItemsSummary = p.Items.Count == 0 ? null : new PropertyDtoItemsSummary
                {
                    TotalItems = p.Items.Count,
                },
                ScansSummary = null,
                SuggestionsSummary = p.Suggestions.Count == 0 ? null : new PropertyDtoSuggestionsSummary
                {
                    TotalSuggestions = p.Suggestions.Count,
                    TotalApprovedSuggestions = p.Suggestions.Count(s => s.Status == PropertySuggestionStatus.APPROVED),
                    TotalPendingSuggestions = p.Suggestions.Count(s => s.Status == PropertySuggestionStatus.PENDING),
                    TotalDeclinedSuggestions = p.Suggestions.Count(s => s.Status == PropertySuggestionStatus.DECLINED)
                }
            };
        }
    }
}
