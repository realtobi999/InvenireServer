using InvenireServer.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InvenireServer.Infrastructure.Persistence.Configurations.Organizations;

public class OrganizationTypeConfiguration : IEntityTypeConfiguration<Organization>
{
    public void Configure(EntityTypeBuilder<Organization> builder)
    {
        // Properties.
        builder.Property(o => o.Id)
            .HasColumnName("id")
            .IsRequired();
        builder.HasKey(o => o.Id);

        builder.Property(o => o.Name)
            .HasColumnName("name")
            .IsRequired();
        builder.HasIndex(o => o.Name)
            .IsUnique();

        builder.Property(o => o.CreatedAt)
            .HasColumnName("created_at")
            .IsRequired();

        builder.Property(o => o.LastUpdatedAt)
            .HasColumnName("last_updated_at");

        // Relationships.
        builder.HasOne(o => o.Admin)
            .WithOne()
            .HasForeignKey<Admin>(a => a.OrganizationId);

        builder.HasMany(o => o.Employees)
            .WithOne()
            .HasForeignKey(e => e.OrganizationId);

        builder.HasMany(o => o.Invitations)
            .WithOne()
            .HasForeignKey(i => i.OrganizationId);
    }
}