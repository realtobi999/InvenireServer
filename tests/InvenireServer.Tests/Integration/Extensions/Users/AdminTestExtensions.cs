using InvenireServer.Application.Cqrs.Admins.Commands.Register;
using InvenireServer.Domain.Entities.Users;

namespace InvenireServer.Tests.Integration.Extensions.Users;

public static class AdminTestExtensions
{
    public static RegisterAdminCommand ToRegisterAdminDto(this Admin admin)
    {
        var dto = new RegisterAdminCommand
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