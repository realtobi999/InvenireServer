using System.Linq.Expressions;
using System.Text.Json.Serialization;
using InvenireServer.Domain.Entities.Users;
using InvenireServer.Application.Dtos.Properties;

namespace InvenireServer.Application.Dtos.Employees;

public record EmployeeDto
{
    [JsonPropertyName("id")]
    public required Guid Id { get; init; }

    [JsonPropertyName("organization_id")]
    public required Guid? OrganizationId { get; init; }

    [JsonPropertyName("name")]
    public required string Name { get; init; }

    [JsonPropertyName("email_address")]
    public required string EmailAddress { get; init; }

    [JsonPropertyName("created_at")]
    public required DateTimeOffset CreatedAt { get; init; }

    [JsonPropertyName("last_updated_at")]
    public required DateTimeOffset? LastUpdatedAt { get; init; }

    [JsonPropertyName("assigned_items")]
    public required List<PropertyItemDto> AssignedItems { get; set; } = [];

    [JsonPropertyName("property_suggestions")]
    public required List<PropertySuggestionDto> Suggestions { get; set; } = [];

    public static Expression<Func<Employee, EmployeeDto>> FromEmployeeSelector
    {
        get
        {
            return e => new EmployeeDto
            {
                Id = e.Id,
                OrganizationId = e.OrganizationId,
                Name = e.Name,
                EmailAddress = e.EmailAddress,
                CreatedAt = e.CreatedAt,
                LastUpdatedAt = e.LastUpdatedAt,
                AssignedItems = e.AssignedItems.Select(i => new PropertyItemDto
                {
                    Id = i.Id,
                    PropertyId = i.PropertyId,
                    EmployeeId = i.EmployeeId,
                    InventoryNumber = i.InventoryNumber,
                    RegistrationNumber = i.RegistrationNumber,
                    Name = i.Name,
                    Price = i.Price,
                    SerialNumber = i.SerialNumber,
                    DateOfPurchase = i.DateOfPurchase,
                    DateOfSale = i.DateOfSale,
                    Location = new PropertyItemLocationDto
                    {
                        Room = i.Location.Room,
                        Building = i.Location.Building,
                        AdditionalNote = i.Location.AdditionalNote
                    },
                    Description = i.Description,
                    DocumentNumber = i.DocumentNumber,
                    CreatedAt = i.CreatedAt,
                    LastUpdatedAt = i.LastUpdatedAt
                }).ToList(),
                Suggestions = e.Suggestions.Select(s => new PropertySuggestionDto
                {
                    Id = s.Id,
                    PropertyId = s.PropertyId,
                    EmployeeId = s.EmployeeId,
                    Name = s.Name,
                    Description = s.Description,
                    Feedback = s.Feedback,
                    Status = s.Status,
                    CreatedAt = s.CreatedAt,
                    ResolvedAt = s.ResolvedAt,
                    LastUpdatedAt = s.LastUpdatedAt
                }).ToList()
            };
        }
    }
}
