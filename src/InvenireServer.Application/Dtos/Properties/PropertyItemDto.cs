using System.Text.Json.Serialization;
using InvenireServer.Application.Attributes;
using InvenireServer.Application.Dtos.Employees;
using InvenireServer.Domain.Entities.Users;

namespace InvenireServer.Application.Dtos.Properties;

[JsonResponse]
public record PropertyItemDto
{
    [JsonPropertyName("id")]
    public Guid Id { get; init; }

    [JsonPropertyName("property_id")]
    public Guid? PropertyId { get; set; }

    [JsonPropertyName("employee_id")]
    public Guid? EmployeeId { get; set; }

    [JsonPropertyName("inventory_number")]
    public string? InventoryNumber { get; set; }

    [JsonPropertyName("registration_number")]
    public string? RegistrationNumber { get; set; }

    [JsonPropertyName("name")]
    public string? Name { get; set; }

    [JsonPropertyName("price")]
    public double Price { get; set; }

    [JsonPropertyName("serial_number")]
    public string? SerialNumber { get; set; }

    [JsonPropertyName("date_of_purchase")]
    public DateTimeOffset DateOfPurchase { get; set; }

    [JsonPropertyName("date_of_sale")]
    public DateTimeOffset? DateOfSale { get; set; }

    [JsonPropertyName("location")]
    public PropertyItemLocationDto? Location { get; set; }

    [JsonPropertyName("description")]
    public string? Description { get; set; }

    [JsonPropertyName("document_number")]
    public string? DocumentNumber { get; set; }

    [JsonPropertyName("created_at")]
    public DateTimeOffset CreatedAt { get; init; }

    [JsonPropertyName("last_updated_at")]
    public DateTimeOffset? LastUpdatedAt { get; set; }

    [JsonPropertyName("last_code_generated_at")]
    public DateTimeOffset? LastCodeGeneratedAt { get; set; }

    [JsonPropertyName("last_scanned_at")]
    public DateTimeOffset? LastScannedAt { get; set; }

    // Additional query specific properties.

    [JsonPropertyName("employee")]
    public EmployeeDto? Employee { get; set; }
}

[JsonResponse]
public record PropertyItemLocationDto
{
    [JsonPropertyName("room")]
    public string? Room { get; set; }

    [JsonPropertyName("building")]
    public string? Building { get; set; }

    [JsonPropertyName("additional_note")]
    public string? AdditionalNote { get; set; }
}
