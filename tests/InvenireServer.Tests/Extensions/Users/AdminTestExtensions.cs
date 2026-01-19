using InvenireServer.Application.Core.Admins.Commands.Register;
using InvenireServer.Domain.Entities.Users;
using InvenireServer.Infrastructure.Persistence;

namespace InvenireServer.Tests.Extensions.Users;

/// <summary>
/// Provides test extensions for <see cref="Admin"/>.
/// </summary>
public static class AdminTestExtensions
{
    /// <summary>
    /// Creates a <see cref="RegisterAdminCommand"/> from an admin instance.
    /// </summary>
    /// <param name="admin">Source admin.</param>
    /// <returns>Register admin command.</returns>
    public static RegisterAdminCommand ToRegisterAdminCommand(this Admin admin)
    {
        var dto = new RegisterAdminCommand
        {
            Id = admin.Id,
            FirstName = admin.FirstName,
            LastName = admin.LastName,
            EmailAddress = admin.EmailAddress,
            Password = admin.Password,
            PasswordConfirm = admin.Password
        };

        return dto;
    }

    /// <summary>
    /// Marks the admin as verified in the database.
    /// </summary>
    /// <param name="admin">Admin to verify.</param>
    /// <param name="context">Database context to update.</param>
    public static void SetAsVerified(this Admin admin, InvenireServerContext context)
    {
        context.Admins.FirstOrDefault(a => a.Id == admin.Id)!.Verify();
        context.SaveChanges();
    }
}
