using InvenireServer.Application.Interfaces.Managers;
using InvenireServer.Tests.Fakers.Common;

namespace InvenireServer.Tests.Unit.Helpers;

/// <summary>
/// Base fixture for command handler tests.
/// </summary>
public abstract class CommandHandlerTester
{
    protected readonly Faker _faker;
    protected readonly IJwtManager _jwt;
    protected readonly Mock<IEmailManager> _email;
    protected readonly Mock<IRepositoryManager> _repositories;

    protected CommandHandlerTester()
    {
        _jwt = JwtManagerFaker.Initiate();
        _faker = new Faker();
        _email = new Mock<IEmailManager>();
        _repositories = new Mock<IRepositoryManager>();
    }
}
