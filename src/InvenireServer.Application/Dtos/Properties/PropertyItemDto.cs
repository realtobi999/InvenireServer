using System.Text.Json.Serialization;
using InvenireServer.Application.Attributes;

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
    public required string DocumentNumber { get; set; }

    [JsonPropertyName("created_at")]
    public required DateTimeOffset CreatedAt { get; init; }

    [JsonPropertyName("last_updated_at")]
    public required DateTimeOffset? LastUpdatedAt { get; set; }
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

[JsonResponse]
public record PropertyItemsSummary
{
    [JsonPropertyName("total_items")]
    public required int TotalItems { get; set; }

    [JsonPropertyName("total_value")]
    public required double TotalValue { get; set; }
}