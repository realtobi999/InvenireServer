using System.Text.Json.Serialization;
using InvenireServer.Application.Attributes;
using InvenireServer.Domain.Entities.Common;

namespace InvenireServer.Application.Core.Properties.Items.Commands.Create;

[JsonRequest]
public record CreatePropertyItemsCommand : IRequest
{
    [JsonPropertyName("items")]
    public required List<CreatePropertyItemCommand> Items { get; init; }

    [JsonIgnore]
    public Jwt? Jwt { get; init; }
}

[JsonRequest]
public record CreatePropertyItemCommand
{
    [JsonPropertyName("id")]
    public Guid? Id { get; init; }

    [JsonPropertyName("inventory_number")]
    public required string InventoryNumber { get; init; }

    [JsonPropertyName("registration_number")]
    public required string RegistrationNumber { get; init; }

    [JsonPropertyName("name")]
    public required string Name { get; init; }

    [JsonPropertyName("price")]
    public required double Price { get; init; }

    [JsonPropertyName("serial_number")]
    public string? SerialNumber { get; init; }

    [JsonPropertyName("date_of_purchase")]
    public required DateTimeOffset DateOfPurchase { get; init; }

    [JsonPropertyName("date_of_sale")]
    public DateTimeOffset? DateOfSale { get; init; }

    [JsonPropertyName("location")]
    public required CreatePropertyItemCommandLocation Location { get; init; }

    [JsonPropertyName("description")]
    public string? Description { get; init; }

    [JsonPropertyName("document_number")]
    public string? DocumentNumber { get; init; }

    [JsonPropertyName("employee_id")]
    public Guid? EmployeeId { get; init; }
}

[JsonRequest]
public record CreatePropertyItemCommandLocation
{
    [JsonPropertyName("room")]
    public required string Room { get; init; }

    [JsonPropertyName("building")]
    public required string Building { get; init; }

    [JsonPropertyName("additional_note")]
    public string? AdditionalNote { get; init; }
}