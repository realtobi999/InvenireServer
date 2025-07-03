using InvenireServer.Domain.Entities.Properties;
using InvenireServer.Domain.Entities.Users;

namespace InvenireServer.Tests.Integration.Fakers.Properties;

public class PropertyItemFaker : Faker<PropertyItem>
{
    public PropertyItemFaker()
    {
        RuleFor(p => p.Id, f => Guid.NewGuid());
        RuleFor(p => p.InventoryNumber, f => f.Commerce.Ean13());
        RuleFor(p => p.RegistrationNumber, f => f.Random.AlphaNumeric(10));
        RuleFor(p => p.Name, f => f.Commerce.ProductName());
        RuleFor(p => p.Price, f => (double)f.Finance.Amount(1, 10000));
        RuleFor(p => p.SerialNumber, f => f.Random.Bool(0.7f) ? f.Commerce.Ean13() : null);
        RuleFor(p => p.DateOfPurchase, f => f.Date.Past(5));
        RuleFor(p => p.DateOfSale, (f, p) => f.Random.Bool(0.3f) ? f.Date.Between(p.DateOfPurchase.DateTime, DateTimeOffset.UtcNow.DateTime) : null);
        RuleFor(p => p.Description, f => f.Random.Bool(0.5f) ? f.Commerce.ProductDescription() : null);
        RuleFor(p => p.DocumentNumber, f => f.Random.AlphaNumeric(15));
        RuleFor(p => p.CreatedAt, f => DateTimeOffset.UtcNow);
        RuleFor(p => p.LastUpdatedAt, f => f.Date.RecentOffset(30));
    }

    public PropertyItemFaker(Property property, Employee? employee = null) : this()
    {
        RuleFor(p => p.PropertyId, property.Id);
        RuleFor(p => p.EmployeeId, employee != null ? employee.Id : null);
    }
}

