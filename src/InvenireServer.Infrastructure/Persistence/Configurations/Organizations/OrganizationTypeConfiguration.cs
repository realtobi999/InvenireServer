using InvenireServer.Domain.Entities.Organizations;
using InvenireServer.Domain.Entities.Properties;
using InvenireServer.Domain.Entities.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InvenireServer.Infrastructure.Persistence.Configurations.Organizations;

/// <summary>
/// Configures the <see cref="Organization"/> entity.
/// </summary>
public class OrganizationTypeConfiguration : IEntityTypeConfiguration<Organization>
{
    /// <summary>
    /// Configures the <see cref="Organization"/> entity.
    /// </summary>
    /// <param name="builder">Builder used to configure the entity.</param>
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
            .HasForeignKey<Admin>(a => a.OrganizationId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne(o => o.Property)
            .WithOne()
            .HasForeignKey<Property>(p => p.OrganizationId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(o => o.Employees)
            .WithOne()
            .HasForeignKey(e => e.OrganizationId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasMany(o => o.Invitations)
            .WithOne()
            .HasForeignKey(i => i.OrganizationId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
