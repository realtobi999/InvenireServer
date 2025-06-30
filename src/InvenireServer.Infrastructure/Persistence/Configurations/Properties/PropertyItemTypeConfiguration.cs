using InvenireServer.Domain.Entities.Properties;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InvenireServer.Infrastructure.Persistence.Configurations.Properties;

public class PropertyItemTypeConfiguration : IEntityTypeConfiguration<PropertyItem>
{
    public void Configure(EntityTypeBuilder<PropertyItem> builder)
    {
        // Properties.
        builder.Property(i => i.Id)
            .HasColumnName("id")
            .IsRequired();
        builder.HasKey(i => i.Id);

        builder.Property(i => i.Name)
            .HasColumnName("name")
            .IsRequired();

        builder.Property(i => i.Description)
            .HasColumnName("description")
            .IsRequired();

        builder.Property(i => i.CreatedAt)
            .HasColumnName("created_at")
            .IsRequired();

        builder.Property(i => i.LastUpdatedAt)
            .HasColumnName("last_updated_at");
    }
}
