using InvenireServer.Application.Core.Properties.Items.Commands.Create;
using InvenireServer.Domain.Entities.Properties;

namespace InvenireServer.Tests.Integration.Extensions.Properties;

public static class PropertyItemTestExtensions
{
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
            Description = property.Description,
            DocumentNumber = property.DocumentNumber,
            EmployeeId = property.EmployeeId
        };

        return dto;
    }
}