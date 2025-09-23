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

        builder.Property(c => c.Status)
            .HasColumnName("status")
            .IsRequired();

        builder.Property(c => c.CreatedAt)
            .HasColumnName("created_at")
            .IsRequired();

        builder.Property(c => c.CompletedAt)
            .HasColumnName("completed_at");

        builder.Property(c => c.LastUpdatedAt)
            .HasColumnName("last_updated_at");

        builder.Property(i => i.PropertyId)
            .HasColumnName("property_id");
    }
}

public class PropertyScanPropertyItemTypeConfiguration : IEntityTypeConfiguration<PropertyScanPropertyItem>
{
    public void Configure(EntityTypeBuilder<PropertyScanPropertyItem> builder)
    {

        // Properties.

        builder.Property(x => x.PropertyScanId)
            .HasColumnName("property_scan_id")
            .IsRequired();

        builder.Property(x => x.PropertyItemId)
            .HasColumnName("property_item_id")
            .IsRequired();

        builder.HasKey(c => new { c.PropertyScanId, c.PropertyItemId });

        builder.Property(x => x.IsScanned)
            .HasColumnName("is_scanned")
            .IsRequired();

        // Relationships.

        builder.HasOne<PropertyScan>()
            .WithMany(p => p.ScannedItems)
            .HasForeignKey(x => x.PropertyScanId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne<PropertyItem>()
            .WithMany()
            .HasForeignKey(x => x.PropertyItemId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
