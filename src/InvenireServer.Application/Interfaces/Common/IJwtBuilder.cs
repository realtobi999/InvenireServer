using System.Security.Claims;
using InvenireServer.Domain.Entities.Common;

namespace InvenireServer.Application.Interfaces.Common;

public interface IJwtBuilder
{
    Jwt Build(List<Claim> claims);
}