using InvenireServer.Domain.Entities.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InvenireServer.Infrastructure.Persistence.Configurations.Users;

public class AdminTypeConfiguration : IEntityTypeConfiguration<Admin>
{
    public void Configure(EntityTypeBuilder<Admin> builder)
    {
        // Properties.
        builder.Property(a => a.Id)
            .HasColumnName("id")
            .IsRequired();
        builder.HasKey(a => a.Id);

        builder.Property(a => a.OrganizationId)
            .HasColumnName("organization_id");

        builder.Property(a => a.Name)
            .HasColumnName("name")
            .HasMaxLength(Admin.MAX_NAME_LENGTH)
            .IsRequired();

        builder.Property(a => a.Password)
            .HasColumnName("password_hash")
            .IsRequired();

        builder.Property(a => a.EmailAddress)
            .HasColumnName("email_address")
            .HasMaxLength(Admin.MAX_EMAIL_ADDRESS_LENGTH)
            .IsRequired();
        builder.HasIndex(a => a.EmailAddress)
            .IsUnique();

        builder.Property(a => a.IsVerified)
            .HasColumnName("is_verified")
            .IsRequired();

        builder.Property(a => a.CreatedAt)
            .HasColumnName("created_at")
            .IsRequired();

        builder.Property(a => a.LastUpdatedAt)
            .HasColumnName("last_updated_at");

        builder.Property(a => a.LastLoginAt)
            .HasColumnName("last_login_at");
    }
}