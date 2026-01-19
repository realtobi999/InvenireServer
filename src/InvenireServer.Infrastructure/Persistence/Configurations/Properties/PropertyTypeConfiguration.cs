using InvenireServer.Domain.Entities.Properties;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InvenireServer.Infrastructure.Persistence.Configurations.Properties;

/// <summary>
/// Configures the <see cref="Property"/> entity.
/// </summary>
public class PropertyTypeConfiguration : IEntityTypeConfiguration<Property>
{
    /// <summary>
    /// Configures the <see cref="Property"/> entity.
    /// </summary>
    /// <param name="builder">Builder used to configure the entity.</param>
    public void Configure(EntityTypeBuilder<Property> builder)
    {
        // Properties.

        builder.Property(p => p.Id)
            .HasColumnName("id")
            .IsRequired();
        builder.HasKey(p => p.Id);

        builder.Property(p => p.CreatedAt)
            .HasColumnName("created_at")
            .IsRequired();

        builder.Property(p => p.LastUpdatedAt)
            .HasColumnName("last_updated_at");

        builder.Property(p => p.OrganizationId)
            .HasColumnName("organization_id");

        // Relationships.

        builder.HasMany(p => p.Items)
            .WithOne()
            .HasForeignKey(i => i.PropertyId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(p => p.Scans)
            .WithOne()
            .HasForeignKey(i => i.PropertyId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(p => p.Suggestions)
            .WithOne()
            .HasForeignKey(s => s.PropertyId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
