using InvenireServer.Application.Interfaces.Common;

namespace InvenireServer.Application.Interfaces.Managers;

public interface IJwtManager
{
    IJwtBuilder Builder { get; }

    IJwtWriter Writer { get; }
}