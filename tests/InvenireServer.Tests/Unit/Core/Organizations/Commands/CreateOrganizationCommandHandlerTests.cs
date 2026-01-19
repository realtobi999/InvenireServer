using InvenireServer.Application.Core.Organizations.Commands.Create;
using InvenireServer.Domain.Entities.Users;
using InvenireServer.Domain.Exceptions.Http;
using InvenireServer.Tests.Fakers.Organizations;
using InvenireServer.Tests.Fakers.Users;
using InvenireServer.Tests.Unit.Helpers;

namespace InvenireServer.Tests.Unit.Core.Organizations.Commands;

/// <summary>
/// Tests for <see cref="CreateOrganizationCommandHandler"/>.
/// </summary>
public class CreateOrganizationCommandHandlerTests : CommandHandlerTester
{
    private readonly CreateOrganizationCommandHandler _handler;

    public CreateOrganizationCommandHandlerTests()
    {
        _handler = new CreateOrganizationCommandHandler(_repositories.Object);
    }

    /// <summary>
    /// Verifies that the handler creates an organization and assigns it to the admin.
    /// </summary>
    /// <returns>Awaitable task representing the test.</returns>
    [Fact]
    public async Task Handle_ThrowsNoException()
    {
        // Prepare.
        var admin = AdminFaker.Fake();
        var organization = OrganizationFaker.Fake();
        var command = new CreateOrganizationCommand
        {
            Jwt = _jwt.Builder.Build([]),
            Name = organization.Name,
            FrontendBaseAddress = "https://www.invenire.com",
        };

        // Prepare - repositories.
        _repositories.Setup(r => r.Admins.GetAsync(command.Jwt)).ReturnsAsync(admin);
        _repositories.Setup(r => r.Admins.Update(admin));
        _repositories.Setup(r => r.Organizations.Create(organization));
        _repositories.Setup(r => r.SaveOrThrowAsync()).Returns(Task.CompletedTask);

        // Act & Assert.
        var action = async () => await _handler.Handle(command, CancellationToken.None);
        await action.Should().NotThrowAsync();

        var result = await action.Invoke();
        result.Organization.Should().NotBeNull();
        result.Organization.Id.Should().NotBeEmpty();
        result.Organization.Name.Should().Be(organization.Name);
        result.Organization.CreatedAt.Should().BeCloseTo(DateTimeOffset.UtcNow, TimeSpan.FromSeconds(1));
        result.Organization.LastUpdatedAt.Should().BeNull();

        // Assert that the admin is assigned to the organization.
        admin.OrganizationId.Should().Be(result.Organization.Id);
    }

    /// <summary>
    /// Verifies that the handler throws when the admin is not found.
    /// </summary>
    /// <returns>Awaitable task representing the test.</returns>
    [Fact]
    public async Task Handle_ThrowsException_WhenAdminIsNotFound()
    {
        // Prepare.
        var admin = AdminFaker.Fake();
        var organization = OrganizationFaker.Fake();
        var command = new CreateOrganizationCommand
        {
            Jwt = _jwt.Builder.Build([]),
            Name = organization.Name,
            FrontendBaseAddress = "https://www.invenire.com",
        };

        // Prepare - repositories.
        _repositories.Setup(r => r.Admins.GetAsync(command.Jwt)).ReturnsAsync((Admin?)null);

        // Act & Assert.
        var action = async () => await _handler.Handle(command, CancellationToken.None);
        await action.Should().ThrowAsync<NotFound404Exception>().WithMessage("The admin was not found in the system.");
    }
}
