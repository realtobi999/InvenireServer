using InvenireServer.Domain.Entities.Users;
using InvenireServer.Domain.Exceptions.Http;

namespace InvenireServer.Domain.Entities.Properties;

public class PropertyItem
{
    // Constants.

    public const int MAX_NAME_LENGTH = 155;

    public const int MAX_DESCRIPTION_LENGTH = 555;

    public const int MAX_IDENTIFICATION_NUMBER_LENGTH = 155;

    // Core properties.

    public required Guid Id { get; set; }

    public required string InventoryNumber { get; set; }

    public required string RegistrationNumber { get; set; }

    public required string Name { get; set; }

    public required double Price { get; set; }

    public required string? SerialNumber { get; set; }

    public required DateTimeOffset DateOfPurchase { get; set; }

    public required DateTimeOffset? DateOfSale { get; set; }

    public required string? Description { get; set; }

    public required string DocumentNumber { get; set; }

    public required DateTimeOffset CreatedAt { get; set; }

    public required DateTimeOffset? LastUpdatedAt { get; set; }

    // Navigational properties.

    public Guid? PropertyId { get; set; }

    public Guid? EmployeeId { get; set; }

    // Methods.

    public void AssignProperty(Property property)
    {
        if (PropertyId is not null) throw new BadRequest400Exception("This item is already part of a another property.");
        PropertyId = property.Id;
    }

    public void AssignEmployee(Employee employee)
    {
        if (EmployeeId is not null) throw new BadRequest400Exception("This item is already assigned to a another employee.");
        EmployeeId = employee.Id;
    }
}
