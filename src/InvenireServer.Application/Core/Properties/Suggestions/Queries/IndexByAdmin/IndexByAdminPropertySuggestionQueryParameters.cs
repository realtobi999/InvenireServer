using InvenireServer.Domain.Entities.Properties;

namespace InvenireServer.Application.Core.Properties.Suggestions.Queries.IndexByAdmin;

public record IndexByAdminPropertySuggestionQueryParameters
{
    public int Limit { get; set; }
    public int Offset { get; set; }

    public bool? Desc { get; set; }
    public string? Order { get; set; }

    public PropertySuggestionStatus? Status { get; set; }

    public string? SearchQuery { get; set; }
}
