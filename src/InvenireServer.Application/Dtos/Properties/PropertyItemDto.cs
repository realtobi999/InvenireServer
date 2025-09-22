using System.Linq.Expressions;
using System.Text.Json.Serialization;
using InvenireServer.Application.Attributes;
using InvenireServer.Application.Dtos.Employees;
using InvenireServer.Domain.Entities.Properties;

namespace InvenireServer.Application.Dtos.Properties;

[JsonResponse]
public record PropertyItemDto
{
    [JsonPropertyName("id")]
    public required Guid Id { get; init; }

    [JsonPropertyName("property_id")]
    public required Guid? PropertyId { get; set; }

    [JsonPropertyName("employee_id")]
    public required Guid? EmployeeId { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("employee")]
    public EmployeeDto? Employee { get; set; }

    [JsonPropertyName("inventory_number")]
    public required string InventoryNumber { get; set; }

    [JsonPropertyName("registration_number")]
    public required string RegistrationNumber { get; set; }

    [JsonPropertyName("name")]
    public required string Name { get; set; }

    [JsonPropertyName("price")]
    public required double Price { get; set; }

    [JsonPropertyName("serial_number")]
    public required string? SerialNumber { get; set; }

    [JsonPropertyName("date_of_purchase")]
    public required DateTimeOffset DateOfPurchase { get; set; }

    [JsonPropertyName("date_of_sale")]
    public required DateTimeOffset? DateOfSale { get; set; }

    [JsonPropertyName("location")]
    public required PropertyItemLocationDto Location { get; set; }

    [JsonPropertyName("description")]
    public required string? Description { get; set; }

    [JsonPropertyName("document_number")]
    public required string? DocumentNumber { get; set; }

    [JsonPropertyName("created_at")]
    public required DateTimeOffset CreatedAt { get; init; }

    [JsonPropertyName("last_updated_at")]
    public required DateTimeOffset? LastUpdatedAt { get; set; }

    public static Expression<Func<PropertyItem, PropertyItemDto>> CoreSelector
    {
        get
        {
            return i => new PropertyItemDto
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
                    AdditionalNote = i.Location.AdditionalNote,
                },
                Description = i.Description,
                DocumentNumber = i.DocumentNumber,
                CreatedAt = i.CreatedAt,
                LastUpdatedAt = i.LastUpdatedAt
            };
        }
    }
}

[JsonResponse]
public record PropertyItemLocationDto
{
    [JsonPropertyName("room")]
    public required string Room { get; set; }

    [JsonPropertyName("building")]
    public required string Building { get; set; }

    [JsonPropertyName("additional_note")]
    public required string? AdditionalNote { get; set; }
}
