using InvenireServer.Application.Dtos.Admins;
using InvenireServer.Domain.Entities;

namespace InvenireServer.Application.Interfaces.Factories.Admins;

public interface IAdminFactory
{
    Admin Create(RegisterAdminDto dto);
}