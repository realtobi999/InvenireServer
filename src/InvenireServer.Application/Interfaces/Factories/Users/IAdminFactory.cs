using InvenireServer.Application.Dtos.Admins;
using InvenireServer.Domain.Entities.Users;

namespace InvenireServer.Application.Interfaces.Factories.Users;

public interface IAdminFactory
{
    Admin Create(RegisterAdminDto dto);
}