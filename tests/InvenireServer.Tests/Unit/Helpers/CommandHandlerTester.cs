using InvenireServer.Application.Interfaces.Managers;
using InvenireServer.Tests.Fakers.Common;

namespace InvenireServer.Tests.Unit.Helpers;

public abstract class CommandHandlerTester
{
    protected readonly IJwtManager _jwt;
    protected readonly Mock<IEmailManager> _email;
    protected readonly Mock<IRepositoryManager> _repositories;

    protected CommandHandlerTester()
    {
        _jwt = JwtManagerFaker.Initiate();
        _email = new Mock<IEmailManager>();
        _repositories = new Mock<IRepositoryManager>();
    }
}