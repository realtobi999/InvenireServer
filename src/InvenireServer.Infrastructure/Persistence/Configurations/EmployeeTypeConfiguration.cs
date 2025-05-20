using InvenireServer.Domain.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InvenireServer.Infrastructure.Persistence.Configurations;

public class EmployeeTypeConfiguration : IEntityTypeConfiguration<Employee>
{
    public void Configure(EntityTypeBuilder<Employee> builder)
    {
        // Properties.
        builder.Property(e => e.Id)
            .HasColumnName("id")
            .IsRequired();
        builder.Property(e => e.OrganizationId)
            .HasColumnName("organization_id");
        builder.Property(e => e.Name)
            .HasColumnName("name")
            .HasMaxLength(Employee.MAX_NAME_LENGTH)
            .IsRequired();
        builder.Property(e => e.EmailAddress)
            .HasColumnName("email_address")
            .HasMaxLength(Employee.MAX_EMAIL_ADDRESS_LENGTH)
            .IsRequired();
        builder.Property(e => e.Password)
            .HasColumnName("password_hash")
            .IsRequired();
        builder.Property(e => e.UpdatedAt)
            .HasColumnName("updated_at");
        builder.Property(e => e.CreatedAt)
            .HasColumnName("created_at")
            .IsRequired();
    }
}