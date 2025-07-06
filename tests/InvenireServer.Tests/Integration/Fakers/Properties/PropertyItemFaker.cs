using InvenireServer.Domain.Entities.Properties;
using InvenireServer.Domain.Entities.Users;

namespace InvenireServer.Tests.Integration.Fakers.Properties;

public class PropertyItemFaker : Faker<PropertyItem>
{
    public PropertyItemFaker()
    {
        RuleFor(i => i.Id, f => Guid.NewGuid());
        RuleFor(i => i.InventoryNumber, f => f.Commerce.Ean13());
        RuleFor(i => i.RegistrationNumber, f => f.Random.AlphaNumeric(10));
        RuleFor(i => i.Name, f => f.Commerce.ProductName());
        RuleFor(i => i.Price, f => (double)f.Finance.Amount(1, 10000));
        RuleFor(i => i.SerialNumber, f => f.Random.Bool(0.7f) ? f.Commerce.Ean13() : null);
        RuleFor(i => i.DateOfPurchase, f => f.Date.Past(5));
        RuleFor(i => i.DateOfSale, (f, p) => f.Random.Bool(0.3f) ? f.Date.Between(p.DateOfPurchase.DateTime, DateTimeOffset.UtcNow.DateTime) : null);
        RuleFor(i => i.Description, f => f.Random.Bool(0.5f) ? f.Commerce.ProductDescription() : null);
        RuleFor(i => i.DocumentNumber, f => f.Random.AlphaNumeric(15));
        RuleFor(i => i.CreatedAt, f => f.Date.PastOffset(5));
        RuleFor(i => i.LastUpdatedAt, f => f.Date.RecentOffset(30));
    }

    public PropertyItemFaker(Property property, Employee? employee = null) : this()
    {
        RuleFor(i => i.PropertyId, property.Id);
        RuleFor(i => i.EmployeeId, employee?.Id);
    }
}