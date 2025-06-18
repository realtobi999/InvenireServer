using InvenireServer.Application.Dtos.Admins;
using InvenireServer.Application.Interfaces.Factories.Users;
using InvenireServer.Domain.Entities.Users;
using Microsoft.AspNetCore.Identity;

namespace InvenireServer.Application.Factories.Users;

public class AdminFactory : IAdminFactory
{
    public Admin Create(RegisterAdminDto dto)
    {
        // Map the object.
        var admin = new Admin
        {
            Id = dto.Id ?? Guid.NewGuid(),
            Name = dto.Name,
            EmailAddress = dto.EmailAddress,
            Password = dto.Password,
            IsVerified = false,
            CreatedAt = DateTimeOffset.Now,
            LastUpdatedAt = null,
            LastLoginAt = null
        };

        // Hash the password.
        admin.Password = new PasswordHasher<Admin>().HashPassword(admin, admin.Password);

        return admin;
    }
}