using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using InvenireServer.Application.Attributes;
using InvenireServer.Domain.Entities.Common;
using InvenireServer.Domain.Entities.Properties;

namespace InvenireServer.Application.Core.Properties.Items.Commands.Create;

[JsonRequest]
public record CreatePropertyItemsCommand : IRequest
{
    [Required]
    [MinLength(1)]
    [JsonPropertyName("items")]
    public required List<CreatePropertyItemCommand> Items { get; set; }

    [JsonIgnore]
    public Jwt? Jwt { get; set; }

    [JsonIgnore]
    public Guid? PropertyId { get; set; }

    [JsonIgnore]
    public Guid? OrganizationId { get; set; }
}

[JsonRequest]
public record CreatePropertyItemCommand
{
    [JsonPropertyName("id")]
    public Guid? Id { get; set; }

    [Required]
    [JsonPropertyName("inventory_number")]
    [MaxLength(PropertyItem.MAX_IDENTIFICATION_NUMBER_LENGTH)]
    public required string InventoryNumber { get; set; }

    [Required]
    [JsonPropertyName("registration_number")]
    [MaxLength(PropertyItem.MAX_IDENTIFICATION_NUMBER_LENGTH)]
    public required string RegistrationNumber { get; set; }

    [Required]
    [JsonPropertyName("name")]
    [MaxLength(PropertyItem.MAX_NAME_LENGTH)]
    public required string Name { get; set; }

    [Required]
    [PositiveDouble]
    [JsonPropertyName("price")]
    public required double Price { get; set; }

    [JsonPropertyName("serial_number")]
    [MaxLength(PropertyItem.MAX_IDENTIFICATION_NUMBER_LENGTH)]
    public string? SerialNumber { get; set; }

    [Required]
    [JsonPropertyName("date_of_purchase")]
    public required DateTimeOffset DateOfPurchase { get; set; }

    [JsonPropertyName("date_of_sale")]
    public DateTimeOffset? DateOfSale { get; set; }

    [JsonPropertyName("description")]
    [MaxLength(PropertyItem.MAX_DESCRIPTION_LENGTH)]
    public string? Description { get; set; }

    [Required]
    [JsonPropertyName("document_number")]
    [MaxLength(PropertyItem.MAX_IDENTIFICATION_NUMBER_LENGTH)]
    public required string DocumentNumber { get; set; }

    [JsonPropertyName("employee_id")]
    public Guid? EmployeeId { get; set; }
}