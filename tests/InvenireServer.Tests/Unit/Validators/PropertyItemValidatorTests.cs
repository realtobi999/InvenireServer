using InvenireServer.Application.Interfaces.Managers;
using InvenireServer.Application.Validators.Properties;
using InvenireServer.Domain.Entities.Properties;
using InvenireServer.Domain.Entities.Users;
using InvenireServer.Domain.Exceptions.Http;
using InvenireServer.Tests.Integration.Fakers.Properties;
using InvenireServer.Tests.Integration.Fakers.Users;

namespace InvenireServer.Tests.Unit.Validators;

public class PropertyItemValidatorTests
{
    private readonly PropertyItemValidator _validator;
    private readonly Mock<IRepositoryManager> _repositories;

    public PropertyItemValidatorTests()
    {
        _repositories = new Mock<IRepositoryManager>();
        _validator = new PropertyItemValidator(_repositories.Object);
    }

    [Fact]
    public async Task ValidateAsync_ReturnsFalseWhenDateOfSaleIsSetBeforeDateOfPurchase()
    {
        // Prepare.
        var item = new PropertyItemFaker().Generate();

        // Set the date of sale to be before the date of purchase.
        item.DateOfSale = item.DateOfPurchase.AddMonths(-1);

        // Act & Assert.
        var (valid, exception) = await _validator.ValidateAsync(item);

        valid.Should().BeFalse();
        exception.Should().BeOfType<BadRequest400Exception>();
        exception!.Message.Should().Contain(nameof(PropertyItem.DateOfSale));
    }

    [Fact]
    public async Task ValidateAsync_ReturnsFalseWhenDateOfPurchaseIsSetInTheFuture()
    {
        // Prepare.
        var item = new PropertyItemFaker().Generate();

        // Set the date of purchase to be in the future.
        item.DateOfPurchase = DateTimeOffset.UtcNow.AddMonths(1);
        item.DateOfSale = null;

        // Act & Assert.
        var (valid, exception) = await _validator.ValidateAsync(item);

        valid.Should().BeFalse();
        exception.Should().BeOfType<BadRequest400Exception>();
        exception!.Message.Should().Contain(nameof(PropertyItem.DateOfPurchase));
    }

    [Fact]
    public async Task ValidateAsync_ReturnsFalseWhenUpdatedAtIsSetBeforeCreatedAt()
    {
        // Prepare.
        var item = new PropertyItemFaker().Generate();

        // Set the updated at time to be before the created at time.
        item.LastUpdatedAt = item.CreatedAt.AddMonths(-1);

        // Act & Assert.
        var (valid, exception) = await _validator.ValidateAsync(item);

        valid.Should().BeFalse();
        exception.Should().BeOfType<BadRequest400Exception>();
        exception!.Message.Should().Contain(nameof(PropertyItem.LastUpdatedAt));
    }

    [Fact]
    public async Task ValidateAsync_ReturnsFalseWhenCreatedAtIsSetInTheFuture()
    {
        // Prepare.
        var item = new PropertyItemFaker().Generate();

        // Set the created at time to be in the future.
        item.CreatedAt = DateTimeOffset.UtcNow.AddMonths(1);
        item.LastUpdatedAt = null;

        // Act & Assert.
        var (valid, exception) = await _validator.ValidateAsync(item);

        valid.Should().BeFalse();
        exception.Should().BeOfType<BadRequest400Exception>();
        exception!.Message.Should().Contain(nameof(PropertyItem.CreatedAt));
    }

    [Fact]
    public async Task ValidateAsync_ReturnsFalseWhenPropertyNotAssigned()
    {
        // Prepare.
        var item = new PropertyItemFaker().Generate();

        // Act & Assert.
        var (valid, exception) = await _validator.ValidateAsync(item);

        valid.Should().BeFalse();
        exception.Should().BeOfType<BadRequest400Exception>();
        exception!.Message.Should().Contain(nameof(Property));
    }

    [Fact]
    public async Task ValidateAsync_ReturnsFalseWhenPropertyIsNotFound()
    {
        // Prepare.
        var property = new PropertyFaker().Generate();
        var item = new PropertyItemFaker(property).Generate();

        // Mock the repository so it doesnt return the property from the repository.
        _repositories.Setup(r => r.Properties.GetAsync(i => i.Id == item.PropertyId)).ReturnsAsync((Property?)null);

        // Act & Assert.
        var (valid, exception) = await _validator.ValidateAsync(item);

        valid.Should().BeFalse();
        exception.Should().BeOfType<NotFound404Exception>();
        exception!.Message.Should().Contain(nameof(Property));
    }

    [Fact]
    public async Task ValidateAsync_ReturnsFalseWhenEmployeeIsNotFound()
    {
        // Prepare.
        var property = new PropertyFaker().Generate();
        var employee = new EmployeeFaker().Generate();
        var item = new PropertyItemFaker(property, employee).Generate();

        _repositories.Setup(r => r.Properties.GetAsync(i => i.Id == item.PropertyId)).ReturnsAsync(property);
        // Mock the repository so it doesnt return the employee from the repository.
        _repositories.Setup(r => r.Employees.GetAsync(i => i.Id == item.EmployeeId)).ReturnsAsync((Employee?)null);

        // Act & Assert.
        var (valid, exception) = await _validator.ValidateAsync(item);

        valid.Should().BeFalse();
        exception.Should().BeOfType<NotFound404Exception>();
        exception!.Message.Should().Contain(nameof(Employee));
    }

    [Fact]
    public async Task ValidateAsync_ReturnsFalseWhenInventoryNumberIsNotUnique()
    {
        // Prepare.
        var property = new PropertyFaker().Generate();
        var item = new PropertyItemFaker(property).Generate();

        _repositories.Setup(r => r.Properties.GetAsync(i => i.Id == item.PropertyId)).ReturnsAsync(property);
        _repositories.Setup(r => r.Properties.Items.IsInventoryNumberUniqueAsync(item, property)).ReturnsAsync(false);

        // Act & Assert.
        var (valid, exception) = await _validator.ValidateAsync(item);

        valid.Should().BeFalse();
        exception.Should().BeOfType<BadRequest400Exception>();
        exception!.Message.Should().Contain(nameof(PropertyItem.InventoryNumber));
    }

    [Fact]
    public async Task ValidateAsync_ReturnsFalseWhenRegistrationNumberIsNotUnique()
    {
        // Prepare.
        var property = new PropertyFaker().Generate();
        var item = new PropertyItemFaker(property).Generate();

        _repositories.Setup(r => r.Properties.GetAsync(i => i.Id == item.PropertyId)).ReturnsAsync(property);
        _repositories.Setup(r => r.Properties.Items.IsInventoryNumberUniqueAsync(item, property)).ReturnsAsync(true);
        _repositories.Setup(r => r.Properties.Items.IsRegistrationNumberUniqueAsync(item, property)).ReturnsAsync(false);

        // Act & Assert.
        var (valid, exception) = await _validator.ValidateAsync(item);

        valid.Should().BeFalse();
        exception.Should().BeOfType<BadRequest400Exception>();
        exception!.Message.Should().Contain(nameof(PropertyItem.RegistrationNumber));
    }

    [Fact]
    public async Task ValidateAsync_ReturnsFalseWhenDocumentNumberIsNotUnique()
    {
        // Prepare.
        var property = new PropertyFaker().Generate();
        var item = new PropertyItemFaker(property).Generate();

        _repositories.Setup(r => r.Properties.GetAsync(i => i.Id == item.PropertyId)).ReturnsAsync(property);
        _repositories.Setup(r => r.Properties.Items.IsInventoryNumberUniqueAsync(item, property)).ReturnsAsync(true);
        _repositories.Setup(r => r.Properties.Items.IsRegistrationNumberUniqueAsync(item, property)).ReturnsAsync(true);
        _repositories.Setup(r => r.Properties.Items.IsDocumentNumberUniqueAsync(item, property)).ReturnsAsync(false);

        // Act & Assert.
        var (valid, exception) = await _validator.ValidateAsync(item);

        valid.Should().BeFalse();
        exception.Should().BeOfType<BadRequest400Exception>();
        exception!.Message.Should().Contain(nameof(PropertyItem.DocumentNumber));
    }

    [Fact]
    public async Task ValidateAsync_ReturnsFalseWhenSerialNumberIsNotUnique()
    {
        // Prepare.
        var property = new PropertyFaker().Generate();
        var item = new PropertyItemFaker(property).Generate();

        item.SerialNumber = Guid.NewGuid().ToString(); // Assign the serial number

        _repositories.Setup(r => r.Properties.GetAsync(i => i.Id == item.PropertyId)).ReturnsAsync(property);
        _repositories.Setup(r => r.Properties.Items.IsInventoryNumberUniqueAsync(item, property)).ReturnsAsync(true);
        _repositories.Setup(r => r.Properties.Items.IsRegistrationNumberUniqueAsync(item, property)).ReturnsAsync(true);
        _repositories.Setup(r => r.Properties.Items.IsDocumentNumberUniqueAsync(item, property)).ReturnsAsync(true);
        _repositories.Setup(r => r.Properties.Items.IsSerialNumberUniqueAsync(item, property)).ReturnsAsync(false);

        // Act & Assert.
        var (valid, exception) = await _validator.ValidateAsync(item);

        valid.Should().BeFalse();
        exception.Should().BeOfType<BadRequest400Exception>();
        exception!.Message.Should().Contain(nameof(PropertyItem.SerialNumber));
    }
}
