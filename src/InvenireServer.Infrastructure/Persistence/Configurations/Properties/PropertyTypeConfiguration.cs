using InvenireServer.Domain.Entities.Properties;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InvenireServer.Infrastructure.Persistence.Configurations.Properties;

public class PropertyTypeConfiguration : IEntityTypeConfiguration<Property>
{
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

        // Relationships.
        builder.HasMany(p => p.Items)
            .WithOne()
            .HasForeignKey(i => i.PropertyId);
    }
}
