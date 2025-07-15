using InvenireServer.Domain.Entities.Properties;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InvenireServer.Infrastructure.Persistence.Configurations.Properties;

public class PropertyScanTypeConfiguration : IEntityTypeConfiguration<PropertyScan>
{
    public void Configure(EntityTypeBuilder<PropertyScan> builder)
    {
        // Properties.

        builder.Property(c => c.Id)
            .HasColumnName("id")
            .IsRequired();
        builder.HasKey(c => c.Id);

        builder.Property(c => c.Name)
            .HasColumnName("name")
            .IsRequired()
            .HasMaxLength(PropertyScan.MAX_NAME_LENGTH);

        builder.Property(c => c.Description)
            .HasColumnName("description")
            .HasMaxLength(PropertyScan.MAX_DESCRIPTION_LENGTH);

        builder.Property(c => c.CreatedAt)
            .HasColumnName("created_at")
            .IsRequired();

        builder.Property(c => c.LastUpdatedAt)
            .HasColumnName("last_updated_at");

        // Relationships.

        builder.HasMany(c => c.ScannedItems)
            .WithMany()
            .UsingEntity<Dictionary<string, object>>("PropertyCheckPropertyItem", i => i.HasOne<PropertyItem>().WithMany().HasForeignKey("PropertyItemId"), c => c.HasOne<PropertyScan>().WithMany().HasForeignKey("PropertyCheckId"));
    }
}
