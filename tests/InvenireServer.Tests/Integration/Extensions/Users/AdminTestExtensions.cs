using InvenireServer.Application.Core.Admins.Commands.Register;
using InvenireServer.Domain.Entities.Users;
using InvenireServer.Infrastructure.Persistence;

namespace InvenireServer.Tests.Integration.Extensions.Users;

public static class AdminTestExtensions
{
    public static RegisterAdminCommand ToRegisterAdminCommand(this Admin admin)
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

    public static void SetAsVerified(this Admin admin, InvenireServerContext context)
    {
        context.Admins.FirstOrDefault(a => a.Id == admin.Id)!.Verify();
        context.SaveChanges();
    }
}