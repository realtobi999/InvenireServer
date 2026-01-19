using InvenireServer.Application.Core.Properties.Scans.Commands.Create;
using InvenireServer.Domain.Entities.Organizations;
using InvenireServer.Domain.Entities.Properties;
using InvenireServer.Domain.Entities.Users;
using InvenireServer.Domain.Exceptions.Http;
using InvenireServer.Tests.Fakers.Organizations;
using InvenireServer.Tests.Fakers.Properties;
using InvenireServer.Tests.Fakers.Users;
using InvenireServer.Tests.Unit.Helpers;

namespace InvenireServer.Tests.Unit.Core.Properties.Scans.Commands;

/// <summary>
/// Tests for <see cref="CreatePropertyScanCommandHandler"/>.
/// </summary>
public class CreatePropertyScanCommandHandlerTests : CommandHandlerTester
{
    private readonly CreatePropertyScanCommandHandler _handler;

    public CreatePropertyScanCommandHandlerTests()
    {
        _handler = new CreatePropertyScanCommandHandler(_repositories.Object);
    }

    /// <summary>
    /// Verifies that the handler creates a scan when no active scan exists.
    /// </summary>
    /// <returns>Awaitable task representing the test.</returns>
    [Fact]
    public async Task Handle_ThrowsNoException()
    {
        // Prepare.
        var admin = AdminFaker.Fake();
        var organization = OrganizationFaker.Fake();
        var property = PropertyFaker.Fake();
        var command = new CreatePropertyScanCommand
        {
            Name = _faker.Lorem.Sentence(),
            Description = _faker.Lorem.Paragraph(),
            Jwt = _jwt.Builder.Build([]),
        };

        // Prepare - repositories.
        _repositories.Setup(r => r.Admins.GetAsync(command.Jwt)).ReturnsAsync(admin);
        _repositories.Setup(r => r.Organizations.GetForAsync(admin)).ReturnsAsync(organization);
        _repositories.Setup(r => r.Properties.GetForAsync(organization)).ReturnsAsync(property);
        _repositories.Setup(r => r.Properties.Scans.IndexInProgressForAsync(property)).ReturnsAsync(Array.Empty<PropertyScan>());
        _repositories.Setup(r => r.Properties.Scans.ExecuteCreateAsync(It.IsAny<PropertyScan>())).Returns(Task.CompletedTask);
        _repositories.Setup(r => r.Properties.Scans.RegisterItemsAsync(It.IsAny<PropertyScan>())).Returns(Task.CompletedTask);

        // Act & Assert.
        var action = async () => await _handler.Handle(command, CancellationToken.None);
        await action.Should().NotThrowAsync();

        var result = await action.Invoke();
        result.Scan.Should().NotBeNull();
        result.Scan.Id.Should().NotBeEmpty();
        result.Scan.Name.Should().Be(command.Name);
        result.Scan.Description.Should().Be(command.Description);
        result.Scan.Status.Should().Be(PropertyScanStatus.IN_PROGRESS);
        result.Scan.CreatedAt.Should().BeCloseTo(DateTimeOffset.UtcNow, TimeSpan.FromSeconds(1));
        result.Scan.CompletedAt.Should().BeNull();
        result.Scan.LastUpdatedAt.Should().BeNull();
        result.Scan.PropertyId.Should().Be(property.Id);
    }

    /// <summary>
    /// Verifies that the handler uses the provided scan id.
    /// </summary>
    /// <returns>Awaitable task representing the test.</returns>
    [Fact]
    public async Task Handle_ThrowsNoException_WhenIdIsProvided()
    {
        // Prepare.
        var admin = AdminFaker.Fake();
        var organization = OrganizationFaker.Fake();
        var property = PropertyFaker.Fake();
        var command = new CreatePropertyScanCommand
        {
            Id = Guid.NewGuid(),
            Name = _faker.Lorem.Sentence(),
            Description = _faker.Lorem.Paragraph(),
            Jwt = _jwt.Builder.Build([]),
        };

        // Prepare - repositories.
        _repositories.Setup(r => r.Admins.GetAsync(command.Jwt)).ReturnsAsync(admin);
        _repositories.Setup(r => r.Organizations.GetForAsync(admin)).ReturnsAsync(organization);
        _repositories.Setup(r => r.Properties.GetForAsync(organization)).ReturnsAsync(property);
        _repositories.Setup(r => r.Properties.Scans.IndexInProgressForAsync(property)).ReturnsAsync(Array.Empty<PropertyScan>());
        _repositories.Setup(r => r.Properties.Scans.ExecuteCreateAsync(It.IsAny<PropertyScan>())).Returns(Task.CompletedTask);
        _repositories.Setup(r => r.Properties.Scans.RegisterItemsAsync(It.IsAny<PropertyScan>())).Returns(Task.CompletedTask);

        // Act & Assert.
        var action = async () => await _handler.Handle(command, CancellationToken.None);
        await action.Should().NotThrowAsync();

        var result = await action.Invoke();
        result.Scan.Id.Should().Be(command.Id.ToString());
        result.Scan.PropertyId.Should().Be(property.Id);
    }

    /// <summary>
    /// Verifies that the handler throws when the admin is not found.
    /// </summary>
    /// <returns>Awaitable task representing the test.</returns>
    [Fact]
    public async Task Handle_ThrowsException_WhenAdminIsNotFound()
    {
        // Prepare.
        var command = new CreatePropertyScanCommand
        {
            Name = _faker.Lorem.Sentence(),
            Description = _faker.Lorem.Paragraph(),
            Jwt = _jwt.Builder.Build([]),
        };

        // Prepare - repositories.
        _repositories.Setup(r => r.Admins.GetAsync(command.Jwt)).ReturnsAsync((Admin?)null);

        // Act & Assert.
        var action = async () => await _handler.Handle(command, CancellationToken.None);
        await action.Should().ThrowAsync<NotFound404Exception>().WithMessage("The admin was not found in the system.");
    }

    /// <summary>
    /// Verifies that the handler throws when the admin has no organization.
    /// </summary>
    /// <returns>Awaitable task representing the test.</returns>
    [Fact]
    public async Task Handle_ThrowsException_WhenOrganizationIsNotCreated()
    {
        // Prepare.
        var admin = AdminFaker.Fake();
        var command = new CreatePropertyScanCommand
        {
            Name = _faker.Lorem.Sentence(),
            Description = _faker.Lorem.Paragraph(),
            Jwt = _jwt.Builder.Build([]),
        };

        // Prepare - repositories.
        _repositories.Setup(r => r.Admins.GetAsync(command.Jwt)).ReturnsAsync(admin);
        _repositories.Setup(r => r.Organizations.GetForAsync(admin)).ReturnsAsync((Organization?)null);

        // Act & Assert.
        var action = async () => await _handler.Handle(command, CancellationToken.None);
        await action.Should().ThrowAsync<BadRequest400Exception>().WithMessage("The admin doesn't own a organization.");
    }

    /// <summary>
    /// Verifies that the handler throws when the organization has no property.
    /// </summary>
    /// <returns>Awaitable task representing the test.</returns>
    [Fact]
    public async Task Handle_ThrowsException_WhenPropertyIsNotFound()
    {
        // Prepare.
        var admin = AdminFaker.Fake();
        var organization = OrganizationFaker.Fake();
        var command = new CreatePropertyScanCommand
        {
            Name = _faker.Lorem.Sentence(),
            Description = _faker.Lorem.Paragraph(),
            Jwt = _jwt.Builder.Build([]),
        };

        // Prepare - repositories.
        _repositories.Setup(r => r.Admins.GetAsync(command.Jwt)).ReturnsAsync(admin);
        _repositories.Setup(r => r.Organizations.GetForAsync(admin)).ReturnsAsync(organization);
        _repositories.Setup(r => r.Properties.GetForAsync(organization)).ReturnsAsync((Property?)null);

        // Act & Assert.
        var action = async () => await _handler.Handle(command, CancellationToken.None);
        await action.Should().ThrowAsync<BadRequest400Exception>().WithMessage("The organization doesn't have a property.");
    }

    /// <summary>
    /// Verifies that the handler throws when an active scan already exists.
    /// </summary>
    /// <returns>Awaitable task representing the test.</returns>
    [Fact]
    public async Task Handle_ThrowsException_WhenActiveScanExists()
    {
        // Prepare.
        var admin = AdminFaker.Fake();
        var organization = OrganizationFaker.Fake();
        var property = PropertyFaker.Fake();

        var scan = PropertyScanFaker.Fake();
        scan.Status = PropertyScanStatus.IN_PROGRESS;

        var command = new CreatePropertyScanCommand
        {
            Name = _faker.Lorem.Sentence(),
            Description = _faker.Lorem.Paragraph(),
            Jwt = _jwt.Builder.Build([]),
        };

        // Prepare - repositories.
        _repositories.Setup(r => r.Admins.GetAsync(command.Jwt)).ReturnsAsync(admin);
        _repositories.Setup(r => r.Organizations.GetForAsync(admin)).ReturnsAsync(organization);
        _repositories.Setup(r => r.Properties.GetForAsync(organization)).ReturnsAsync(property);
        _repositories.Setup(r => r.Properties.Scans.IndexInProgressForAsync(property)).ReturnsAsync([scan]);

        // Act & Assert.
        var action = async () => await _handler.Handle(command, CancellationToken.None);
        await action.Should().ThrowAsync<Conflict409Exception>().WithMessage("The organization already has an active scan.");
    }
}
