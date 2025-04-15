using InvenireServer.Domain.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InvenireServer.Infrastructure.Persistence.Configurations;

public class PropertyScanConfiguration : IEntityTypeConfiguration<PropertyScan>
{
    public void Configure(EntityTypeBuilder<PropertyScan> builder)
    {
        // Properties
        builder.Property(ps => ps.Id)
               .HasColumnName("id")
               .IsRequired();

        // Relationships.
        builder.HasOne<Organization>()
               .WithMany()
               .HasForeignKey(ps => ps.OrganizationId);
        builder.HasOne<Property>()
               .WithMany()
               .HasForeignKey(ps => ps.PropertyId);
        builder.OwnsMany(ps => ps.ScannedItems, ob =>
        {
            // Configure the PropertyItem entity using a builder extension method.
            ob.ConfigurePropertyItem();
        });
    }
}
