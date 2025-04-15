using InvenireServer.Domain.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InvenireServer.Infrastructure.Persistence.Configurations;

public class PropertyTypeConfiguration : IEntityTypeConfiguration<Property>
{
    public void Configure(EntityTypeBuilder<Property> builder)
    {
        // Properties.
        builder.Property(p => p.Id)
               .HasColumnName("id")
               .IsRequired();

        // Relationships.
        builder.HasMany(p => p.Groups)
               .WithOne()
               .HasForeignKey(pg => pg.PropertyId);
    }
}

public class PropertyGroupTypeConfiguration : IEntityTypeConfiguration<PropertyGroup>
{
    public void Configure(EntityTypeBuilder<PropertyGroup> builder)
    {
        // Properties.
        builder.Property(pg => pg.Id)
               .HasColumnName("id")
               .IsRequired();

        // Relationships.
        builder.HasMany(pg => pg.Subs)
               .WithOne()
               .HasForeignKey(pg => pg.ParentGroupId);
        builder.OwnsMany(pg => pg.Items, ob =>
        {
            // Configure the PropertyItem entity using a builder extension method.
            ob.ConfigurePropertyItem();
        });
    }
}

public static class OwnedNavigationBuilderExtensions
{
    /// <summary>
    /// Configures the entity of type PropertyItem.
    /// </summary>
    public static void ConfigurePropertyItem<T>(this OwnedNavigationBuilder<T, PropertyItem> builder) where T : class
    {
        // Properties.
        builder.Property(pi => pi.Id)
               .HasColumnName("id")
               .IsRequired();
    }
}

