using InvenireServer.Domain.Entities;
using InvenireServer.Application.Dtos.Admins;

namespace InvenireServer.Application.Interfaces.Factories.Admins;

public interface IAdminFactory
{
    Admin Create(RegisterAdminDto dto);
}
