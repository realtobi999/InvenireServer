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

        builder.Property(si => si.Id)
            .HasColumnName("id")
            .IsRequired();
        builder.HasKey(si => si.Id);

        builder.Property(si => si.IsScanned)
            .HasColumnName("is_scanned")
            .IsRequired();

        builder.Property(si => si.CreatedAt)
            .HasColumnName("created_at")
            .IsRequired();

        builder.Property(si => si.ScannedAt)
            .HasColumnName("scanned_at");

        builder.Property(si => si.PropertyScanId)
            .HasColumnName("property_scan_id")
            .IsRequired();

        builder.Property(si => si.PropertyItemId)
            .HasColumnName("property_item_id");

        builder.Property(si => si.PropertyItemEmployeeId)
            .HasColumnName("property_item_employee_id");

        // Relationships.

        builder.HasOne<PropertyScan>()
            .WithMany(p => p.ScannedItems)
            .HasForeignKey(si => si.PropertyScanId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne<PropertyItem>()
            .WithMany()
            .HasForeignKey(si => si.PropertyItemId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
