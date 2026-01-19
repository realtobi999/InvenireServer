using InvenireServer.Application.Core.Properties.Items.Commands.Create;
using InvenireServer.Domain.Entities.Properties;

namespace InvenireServer.Tests.Extensions.Properties;

/// <summary>
/// Provides test extensions for <see cref="PropertyItem"/>.
/// </summary>
public static class PropertyItemTestExtensions
{
    /// <summary>
    /// Creates a <see cref="CreatePropertyItemCommand"/> from a property item.
    /// </summary>
    /// <param name="property">Source property item.</param>
    /// <returns>Create property item command.</returns>
    public static CreatePropertyItemCommand ToCreatePropertyItemCommand(this PropertyItem property)
    {
        var dto = new CreatePropertyItemCommand
        {
            Id = property.Id,
            InventoryNumber = property.InventoryNumber,
            RegistrationNumber = property.RegistrationNumber,
            Name = property.Name,
            Price = property.Price,
            SerialNumber = property.SerialNumber,
            DateOfPurchase = property.DateOfPurchase,
            DateOfSale = property.DateOfSale,
            Location = new CreatePropertyItemCommandLocation
            {
                Room = property.Location.Room,
                Building = property.Location.Building,
                AdditionalNote = property.Location.AdditionalNote
            },
            Description = property.Description,
            DocumentNumber = property.DocumentNumber,
            EmployeeId = property.EmployeeId
        };

        return dto;
    }
}
