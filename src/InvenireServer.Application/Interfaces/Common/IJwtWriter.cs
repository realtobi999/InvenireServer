using InvenireServer.Domain.Entities.Common;

namespace InvenireServer.Application.Interfaces.Common;

public interface IJwtWriter
{
    string Write(Jwt jwt);
}