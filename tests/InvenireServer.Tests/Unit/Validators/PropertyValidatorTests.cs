using System.Linq.Expressions;
using InvenireServer.Application.Interfaces.Managers;
using InvenireServer.Application.Validators.Properties;
using InvenireServer.Domain.Entities.Organizations;
using InvenireServer.Domain.Entities.Properties;
using InvenireServer.Domain.Exceptions.Http;
using InvenireServer.Tests.Integration.Fakers.Organizations;
using InvenireServer.Tests.Integration.Fakers.Properties;

namespace InvenireServer.Tests.Unit.Validators;

public class PropertyValidatorTests
{
    private readonly Mock<IRepositoryManager> _repositories;
    private readonly PropertyValidator _validator;

    public PropertyValidatorTests()
    {
        _repositories = new Mock<IRepositoryManager>();
        _validator = new PropertyValidator(_repositories.Object);
    }

    [Fact]
    public async Task ValidateAsync_ReturnsFalseWhenUpdatedAtIsSetBeforeCreatedAt()
    {
        // Prepare.
        var property = new PropertyFaker().Generate();

        // Set the updated at time to be before the created at time.
        property.LastUpdatedAt = property.CreatedAt.AddMonths(-1);

        // Act & Assert.
        var (valid, exception) = await _validator.ValidateAsync(property);

        valid.Should().BeFalse();
        exception.Should().BeOfType<BadRequest400Exception>();
        exception!.Message.Should().Contain(nameof(Property.LastUpdatedAt));
    }

    [Fact]
    public async Task ValidateAsync_ReturnsFalseWhenCreatedAtIsSetInTheFuture()
    {
        // Prepare.
        var property = new PropertyFaker().Generate();

        // Set the created at time to be in the future.
        property.CreatedAt = DateTimeOffset.UtcNow.AddMonths(1);
        property.LastUpdatedAt = null;

        // Act & Assert.
        var (valid, exception) = await _validator.ValidateAsync(property);

        valid.Should().BeFalse();
        exception.Should().BeOfType<BadRequest400Exception>();
        exception!.Message.Should().Contain(nameof(Property.CreatedAt));
    }

    [Fact]
    public async Task ValidateAsync_ReturnsFalseWhenOrganizationIsNotAssigned()
    {
        // Prepare.
        var property = new PropertyFaker().Generate();

        // Act & Assert.
        var (valid, exception) = await _validator.ValidateAsync(property);

        valid.Should().BeFalse();
        exception.Should().BeOfType<BadRequest400Exception>();
        exception!.Message.Should().Contain(nameof(Organization));
    }

    [Fact]
    public async Task ValidateAsync_ReturnsFalseWhenOrganizationIsNotFound()
    {
        // Prepare.
        var organization = new OrganizationFaker().Generate();
        var property = new PropertyFaker(organization).Generate();

        // Mock the repository so it doesnt return the organization from the repository.
        _repositories.Setup(r => r.Organizations.GetAsync(o => o.Id == property.OrganizationId)).ReturnsAsync((Organization?)null);

        // Act & Assert.
        var (valid, exception) = await _validator.ValidateAsync(property);

        valid.Should().BeFalse();
        exception.Should().BeOfType<NotFound404Exception>();
        exception!.Message.Should().Contain(nameof(Organization));
    }

    [Fact]
    public async Task ValidateAsync_ReturnsFalseWhenEmployeeIsNotFound()
    {
        // Prepare.
        var organization = new OrganizationFaker().Generate();
        var property = new PropertyFaker(organization).Generate();

        property.Items.Add(new PropertyItemFaker(property).Generate());

        _repositories.Setup(r => r.Organizations.GetAsync(o => o.Id == property.OrganizationId)).ReturnsAsync(organization);
        // Mock the repository so it doesnt return the item from the repository.
        _repositories.Setup(s => s.Properties.Items.GetAsync(It.IsAny<Expression<Func<PropertyItem, bool>>>())).ReturnsAsync((PropertyItem?)null);

        // Act & Assert.
        var (valid, exception) = await _validator.ValidateAsync(property);

        valid.Should().BeFalse();
        exception.Should().BeOfType<NotFound404Exception>();
        exception!.Message.Should().Contain(nameof(PropertyItem));
    }
}