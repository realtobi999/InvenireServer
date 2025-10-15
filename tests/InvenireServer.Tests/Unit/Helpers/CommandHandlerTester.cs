using InvenireServer.Application.Interfaces.Managers;
using InvenireServer.Infrastructure.Authentication;
using InvenireServer.Tests.Fakers.Common;

namespace InvenireServer.Tests.Unit.Helpers;

public abstract class CommandHandlerTester<THandler>
{
    protected CommandHandlerTester(Func<Mock<IRepositoryManager>, THandler> factory)
    {
        _jwt = JwtManagerFaker.Initiate();
        _repositories = new Mock<IRepositoryManager>();

        _handler = factory(_repositories);
    }

    protected readonly JwtManager _jwt;
    protected readonly Mock<IRepositoryManager> _repositories;

    protected readonly THandler _handler;
}
