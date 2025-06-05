using InvenireServer.Application.Dtos.Admins;
using InvenireServer.Application.Interfaces.Common;
using InvenireServer.Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace InvenireServer.Application.Mappers;

public class AdminMapper : IMapper<Admin, RegisterAdminDto>
{
    private readonly IPasswordHasher<Admin> _hasher;

    public AdminMapper(IPasswordHasher<Admin> hasher)
    {
        _hasher = hasher;
    }

    public Admin Map(RegisterAdminDto dto)
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
            LastLoginAt = null,
        };

        // Hash the password.
        admin.Password = _hasher.HashPassword(admin, admin.Password);

        return admin;
    }
}
