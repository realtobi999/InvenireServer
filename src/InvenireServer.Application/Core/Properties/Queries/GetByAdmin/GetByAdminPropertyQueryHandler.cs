using System.Linq.Expressions;
using InvenireServer.Application.Dtos.Properties;
using InvenireServer.Application.Interfaces.Managers;
using InvenireServer.Domain.Entities.Common.Queries;
using InvenireServer.Domain.Entities.Properties;
using InvenireServer.Domain.Exceptions.Http;

namespace InvenireServer.Application.Core.Properties.Queries.GetByAdmin;

/// <summary>
/// Handler for the query to get a property for an admin.
/// </summary>
public class GetByAdminPropertyQueryHandler : IRequestHandler<GetByAdminPropertyQuery, PropertyDto>
{
    private readonly IRepositoryManager _repositories;

    public GetByAdminPropertyQueryHandler(IRepositoryManager repositories)
    {
        _repositories = repositories;
    }

    /// <summary>
    /// Handles the query to get a property for an admin.
    /// </summary>
    /// <param name="request">Query to handle.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>Awaitable task returning the response.</returns>
    public async Task<PropertyDto> Handle(GetByAdminPropertyQuery request, CancellationToken ct)
    {
        var admin = await _repositories.Admins.GetAsync(request.Jwt) ?? throw new NotFound404Exception("The admin was not found in the system.");
        var organization = await _repositories.Organizations.GetForAsync(admin) ?? throw new BadRequest400Exception("The admin doesn't own a organization.");

        return await _repositories.Properties.GetAsync(new QueryOptions<Property, PropertyDto>
        {
            Selector = PropertyDtoSelector,
            Filtering = new QueryFilteringOptions<Property>
            {
                Filters =
                [
                    p => p.OrganizationId == organization.Id,
                ]
            }
        }) ?? throw new BadRequest400Exception("The organization doesn't have a property.");
    }

    private static Expression<Func<Property, PropertyDto>> PropertyDtoSelector
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
                    TotalValue = p.Items.Sum(i => i.Price),
                    AverageAge = p.Items.Average(i => (DateTimeOffset.UtcNow - i.DateOfPurchase).TotalDays / 365.25),
                    AveragePrice = p.Items.Average(i => i.Price),
                },
                ScansSummary = p.Scans.Count == 0 ? null : new PropertyDtoScansSummary
                {
                    TotalScans = p.Scans.Count,
                    TotalActiveScans = p.Scans.Count(s => s.CompletedAt == null),
                    LastActiveScan = p.Scans.Where(s => s.CompletedAt != null).Max(s => s.CompletedAt)

                },
                SuggestionsSummary = p.Suggestions.Count == 0 ? null : new PropertyDtoSuggestionsSummary
                {
                    TotalSuggestions = p.Suggestions.Count,
                    TotalApprovedSuggestions = p.Suggestions.Count(s => s.Status == PropertySuggestionStatus.APPROVED),
                    TotalPendingSuggestions = p.Suggestions.Count(s => s.Status == PropertySuggestionStatus.PENDING),
                    TotalDeclinedSuggestions = p.Suggestions.Count(s => s.Status == PropertySuggestionStatus.DECLINED)
                },
            };
        }
    }
}
