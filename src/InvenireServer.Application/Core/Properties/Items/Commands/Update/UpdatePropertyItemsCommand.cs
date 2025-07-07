using System.Text.Json.Serialization;
using InvenireServer.Application.Attributes;
using InvenireServer.Domain.Entities.Common;

namespace InvenireServer.Application.Core.Properties.Items.Commands.Update;

[JsonRequest]
public record UpdatePropertyItemsCommand : IRequest
{
    [JsonPropertyName("items")]
    public required List<UpdatePropertyItemCommand> Items { get; set; }

    [JsonIgnore]
    public Jwt? Jwt { get; set; }
}

[JsonRequest]
public record UpdatePropertyItemCommand
{
    [JsonPropertyName("id")]
    public required Guid Id { get; set; }

    [JsonPropertyName("inventory_number")]
    public required string InventoryNumber { get; set; }

    [JsonPropertyName("registration_number")]
    public required string RegistrationNumber { get; set; }

    [JsonPropertyName("name")]
    public required string Name { get; set; }

    [JsonPropertyName("price")]
    public required double Price { get; set; }

    [JsonPropertyName("serial_number")]
    public string? SerialNumber { get; set; }

    [JsonPropertyName("date_of_purchase")]
    public required DateTimeOffset DateOfPurchase { get; set; }

    [JsonPropertyName("date_of_sale")]
    public DateTimeOffset? DateOfSale { get; set; }

    [JsonPropertyName("description")]
    public string? Description { get; set; }

    [JsonPropertyName("document_number")]
    public required string DocumentNumber { get; set; }

    [JsonPropertyName("employee_id")]
    public Guid? EmployeeId { get; set; }
}