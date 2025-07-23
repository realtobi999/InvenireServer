using InvenireServer.Domain.Entities.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InvenireServer.Infrastructure.Persistence.Configurations.Users;

public class EmployeeTypeConfiguration : IEntityTypeConfiguration<Employee>
{
    public void Configure(EntityTypeBuilder<Employee> builder)
    {
        // Properties.

        builder.Property(e => e.Id)
            .HasColumnName("id")
            .IsRequired();
        builder.HasKey(e => e.Id);

        builder.Property(e => e.OrganizationId)
            .HasColumnName("organization_id");

        builder.Property(e => e.Name)
            .HasColumnName("name")
            .HasMaxLength(Employee.MAX_NAME_LENGTH)
            .IsRequired();

        builder.Property(e => e.Password)
            .HasColumnName("password_hash")
            .IsRequired();

        builder.Property(e => e.EmailAddress)
            .HasColumnName("email_address")
            .HasMaxLength(Employee.MAX_EMAIL_ADDRESS_LENGTH)
            .IsRequired();
        builder.HasIndex(e => e.EmailAddress)
            .IsUnique();

        builder.Property(e => e.IsVerified)
            .HasColumnName("is_verified")
            .IsRequired();

        builder.Property(e => e.CreatedAt)
            .HasColumnName("created_at")
            .IsRequired();

        builder.Property(e => e.LastUpdatedAt)
            .HasColumnName("last_updated_at");

        builder.Property(e => e.LastLoginAt)
            .HasColumnName("last_login_at");

        builder.Property(p => p.OrganizationId)
            .HasColumnName("organization_id");

        // Relationships

        builder.HasMany(e => e.AssignedItems)
            .WithOne()
            .HasForeignKey(i => i.EmployeeId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasMany(e => e.Suggestions)
            .WithOne()
            .HasForeignKey(s => s.EmployeeId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}