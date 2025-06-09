using InvenireServer.Application.Dtos.Admins;
using InvenireServer.Domain.Entities;

namespace InvenireServer.Tests.Integration.Extensions;

public static class AdminTestExtensions
{
    public static RegisterAdminDto ToRegisterAdminDto(this Admin admin)
    {
        var dto = new RegisterAdminDto
        {
            Id = admin.Id,
            Name = admin.Name,
            EmailAddress = admin.EmailAddress,
            Password = admin.Password,
            PasswordConfirm = admin.Password
        };

        return dto;
    }
}